using ConsoleApp21.Players.User;
using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;
using ConsoleKeyInfo = System.ConsoleKeyInfo;

GameManager gameManager = new GameManager();
gameManager.users = Users.Load();


while (true)
{
    gameManager.BeginingTheGame();
}


class GameManager
{
    int size = 3;
    public int CountUsers = 2;
    public int turn = 0;
    public List<User> users = new List<User>();
    

    public void BeginingTheGame()
    {
        Console.WriteLine("Welcome to the ULTRA X0" +
                            "\n 1. PvP" +
                            "\n 2. PvE" +
                            "\n 3. Change field's size" +
                            "\n 4. Check Scorebar" +
                            "\n 5. Save Scorebar" +
                            "\n 6. Exit" +
                            "\n 7. Matchmaking");
        
        switch (Convert.ToInt32(Console.ReadLine()))
        {
            case 1:
                while (users.Count < 2) 
                    Users.CreateUser(users);
                
                Console.WriteLine("Are you want to add user? (Press +)");
                if (Console.ReadKey().Key == ConsoleKey.Add)
                    Users.CreateUser(users);
                
                Map map = new Map(size);
                Console.WriteLine("Choose player:");
                StartGame(
                    Users.GetUser(users), 
                    Users.GetUser(users), 
                    map);
                break;
            case 3:
                ChangeSizeOfField();
                break;
            case 4:
                Users.Stats.Print(users);
                break;
            case 5:
                Users.Save(users);
                break;
            // Мой онлайн
            case 7:
                try
                {
                    var client = Network.Client.Connect("62.84.101.222", 8000);
                    Thread.Sleep(500);
                    bool isFindedPlayer = false;
                    bool yourMove = false;
                    client.OnMessage += (client, message) =>
                    {
                        string cmd = message.Split(" ")[0];
                        string[] args = message.Split(" ");
                        switch (cmd)
                        {
                            case "FIND_MATCH_PLAYERS":
                                Console.WriteLine($"Игроков в поиске: {args[1]}");
                                break;
                            case "GAME_START":
                                Console.WriteLine($"Игра началась! №{args[1]}");
                                isFindedPlayer = true;
                                if (args[2] == "MOVE") yourMove = true;
                                else yourMove = false;
                                break;
                        }
    
                    };
                    client.Send("FIND_MATCHES_START");

                    while (isFindedPlayer == false) { }
                    var m = new Map(3);
                
                    if (yourMove) StartGame(new OnlineUser(m, client), new OnlineUserEnemy(m, client), m);
                    else StartGame(new OnlineUserEnemy(m, client),new OnlineUser(m, client),  m);

                }
                catch (Exception e)
                {
                    var defColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Matchmaking is unavailable");
                    Console.ForegroundColor = defColor;
                }
                
                break;
        }
    }
    
    public void StartGame(IPlayer player1, IPlayer player2, Map map)
    {
        for (int i = 0; i < map._size * map._size; i++)
        {
            if (turn % 2 == 0) player1.Move(map);
            else player2.Move(map);
            
            if (Result.AllVariants(map, turn, player1, player2)) break;
            turn++;
        }
    }  // начало игры, передача хода
    
    public void ChangeSizeOfField()
    {
        Console.WriteLine("Enter size");
        if (!int.TryParse(Console.ReadLine(), out int size))
        {
            Console.WriteLine("failed");
        }
        else
        {
            this.size = size;
        }
        
    }
}








