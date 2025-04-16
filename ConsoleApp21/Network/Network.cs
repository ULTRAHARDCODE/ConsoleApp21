using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

public static class Network 
{
    public static string LocalIP
     {
         get
         {
             foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
             {
                 if (netInterface.OperationalStatus == OperationalStatus.Up)
                 {
                     var properties = netInterface.GetIPProperties();
                     foreach (var address in properties.UnicastAddresses)
                     {
                         if (address.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address.Address))
                         {
                             return address.Address.ToString();
                         }
                     }
                 }
             }
             return "";
         }
     }

    private static bool Send(TcpClient client, string message)
    {
        try
        {
            var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message);
                
            stream.Write(data, 0, data.Length);
            stream.Flush();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    private static string Receive(TcpClient client)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    private static async Task<bool> SendAsync(TcpClient client, string message) 
    {
        try
        {
            var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message);
                
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private static async Task<string> ReceiveAsync(TcpClient client)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length)) > 0)
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public static class Events
    {
        public delegate void OnConnect(Client client);
        public delegate void OnDisconnect(Client client);
        public delegate void OnMessage(Client client, string message);
        
    }
    
    public class Client
    {
        private TcpClient tcpClient;
        public event Events.OnMessage OnMessage;
        public event Events.OnMessage OnAllMessage;
        public event Events.OnDisconnect OnDisconnect;
        private Dictionary<string, MethodInfo> methods;

        private bool isConnected = false;
        public bool Connected
        {
            get
            {
                return this.tcpClient.Connected;
            }
        }

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            methods = CheckMethods();
            Task.Run(() => {
                while (true)
                    if (Receive() is null)
                        break;
            });
            isConnected = true;
        }
        
        public static Client Connect(string ip, int port)
        {
            return new Client(new TcpClient(ip, port));
        }
        
        public static Client Connect(TcpListener tcpListener)
        {
            return new Client(tcpListener.AcceptTcpClient());
        }

        public static T Connect<T>(string ip, int port) where T : Client
        {
            var tcp = new TcpClient(ip, port);
            return (T)Activator.CreateInstance(typeof(T), tcp)!;
        }

        public static T Connect<T>(TcpListener tcpListener)
        {
            return (T)Activator.CreateInstance(typeof(T), tcpListener.AcceptTcpClient())!;
        }

        public bool Send(string message)
        {
            return Network.Send(tcpClient, message);
        }

        public string Receive()
        {
            if (!tcpClient.Connected) return null;
            
            var message = Network.Receive(tcpClient);
            if (message is not null)
            {
                var command = message.Split(" ")[0];
                OnAllMessage?.Invoke(this, message);
                
                if (methods.TryGetValue(command, out var method))
                {
                    method.Invoke(this, new object[] { message.Remove(0, command.Length+1) });
                }
                else
                {
                    OnMessage?.Invoke(this, message);
                }
                
                
            }
            else
            {
                OnDisconnect?.Invoke(this);
                tcpClient.Close();
            }
            
            
            return message;
        }

        public void Disconnect()
        {
            tcpClient.Client.Shutdown(SocketShutdown.Both);
            tcpClient.Close(); 
            
            OnDisconnect?.Invoke(this);
        }
        
        private Dictionary<string, MethodInfo> CheckMethods()
        {
            var methodInfos = new Dictionary<string, MethodInfo>();
            var methods = GetType().GetMethods();
         
            foreach (var method in methods)
            {
                var attribute = (Network.RouteMethod?)Attribute.GetCustomAttribute(method, typeof(Network.RouteMethod));
                if (attribute != null) // Метод имеет атрибут RouteMethod
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string))
                    {
                        throw new InvalidOperationException(
                            $"Метод с атрибутом [RouteMethod] {method.Name} должен иметь ровно один параметр типа string!"
                        );
                    }
                    methodInfos.Add(attribute.Name, method);
                }
            }
         
            return methodInfos;
        }
        
        public int latency { get; private set; } = 0;
        private DateTime sendTime;

        public void Pingger(int latency = 10000)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Ping();
                    Task.Delay(latency).Wait();
                }
            });
        }
        public void Ping()
        {
            Send($"PING {latency}");
            sendTime = DateTime.Now;
        }
        
        [RouteMethod("PING")]
        public void Ping(string message)
        {
            Send($"PONG {latency}");
        }
        
        [RouteMethod("PONG")]
        public void Pong(string message)
        {
            latency = DateTime.Now.Millisecond - sendTime.Millisecond;
        }
        
    }
    
    public class RouteMethod : Attribute
    {
        public string Name;

        public RouteMethod(string name)
        {
            Name = name;
        }
    }

    public class Server
    {
        private TcpListener tcpListener;
        private Thread serverThread;
        private CancellationTokenSource cts;
        private CancellationToken token;
        
        public event Events.OnConnect OnConnect;
        public event Events.OnMessage OnMessage;
        public event Events.OnDisconnect OnDisconnect;
        
        public List<Client> Clients = new List<Client>();
        
        public Server(string ip, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ip), port);
        }
        
        public void Start()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            serverThread = new Thread(ServerLoop<Client>);
            serverThread.Start();
        }

        public void Start<T>() where T : Client
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            serverThread = new Thread(ServerLoop<T>);
            serverThread.Start();
        }

        
        public void Stop()
        {
            if (serverThread is null && tcpListener is null) return;
            
            cts.Cancel();
            tcpListener.Stop();
        }
        
        private async void ServerLoop<T>() where T : Client
        {
            tcpListener.Start();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var client = Client.Connect<T>(tcpListener);
                    
                    Clients.Add(client);
                    OnConnect?.Invoke(client);
                    client.OnMessage += (client, message) =>
                    {
                        OnMessage?.Invoke(client, message);
                    };
                    client.OnDisconnect += (client) => { 
                        Clients.Remove(client);
                        OnDisconnect?.Invoke(client);
                    };
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested)
                        break;
                }
            }
        }

        public void Broadcast(string message)
        {
            Parallel.ForEach(Clients.ToArray(), client => 
                client.Send(message)
                );
        }
    }
}