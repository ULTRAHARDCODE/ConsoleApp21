using System.Net.Sockets;

var server = new Network.Server("0.0.0.0", 8000);
server.Start<User>();


server.OnConnect += (client) => {
    client.Pingger(30000);
};
server.OnMessage += (client, message) => {
    User user = (User)client;
    string cmd = message.Split(" ")[0];
    string[] args = message.Split(" ");
    switch (cmd)
    {
        case "FIND_MATCHES_START":
            Manager.userStartFindMatch((User)client);
            break;
        case "FIND_MATCHES_STOP":
            Manager.userStopFindMatch((User)client);
            break;
        case "GAME_MOVE":
            user.gameConnected?.moveGame(user, args[1], args[2], args[3], args[4]);
            break;
        case "GAME_START":
            break;
        case "GAME_END":
            user.gameConnected?.endGame(user);
            break;
        
        case "LOGIN_REQUEST":
            break;
        case "GET_STATS":
            break;
    }
    
    Manager.startGame();
};
server.OnDisconnect += (client) => {
    User user = (User)client;
    
    Manager.userStopFindMatch(user);
    user.gameConnected?.endGame(user);
};

Console.WriteLine("Сервер запущен");

public class User : Network.Client
{
    public Game gameConnected;
    public User(TcpClient client) : base(client) { }
    
}

public static class Manager
{
    public static List<Game> games = new List<Game>();
    public static List<User> playersFindMatches = new List<User>();
    public static void userStartFindMatch(User user)
    {
        if (playersFindMatches.Contains(user))
        {
            Console.WriteLine("Игрок уже в поиске");
            return;
        }
        
        playersFindMatches.Add(user);
        user.Send("FIND_MATCH_PLAYERS "+ playersFindMatches.Count);
        Console.WriteLine("Игрок запустил поиск");
    }

    public static void userStopFindMatch(User user, bool log = true)
    {
        if (playersFindMatches.Contains(user))
        {
            playersFindMatches.Remove(user);
            if (log) Console.WriteLine("Игрок остановил поиск");
        }
    }

    public static void startGame()
    {
        Console.WriteLine("Количество игроков в поиске: "+playersFindMatches.Count);
        if (playersFindMatches.Count >= 2)
        {
            Task.Run(() =>
            {
                var game = new Game(playersFindMatches[0], playersFindMatches[1]);
                Task.Delay(100).Wait();
                games.Add(game);
                game.startGame();
            });
            
        }
    }

    public static int getIDGame(Game game)
    {
        return games.IndexOf(game);
    }
}

public class Game
{
    public int Id;
    public User user1;
    public User user2;
    
    public Game(User user1, User user2)
    {
        Manager.userStopFindMatch(user1,false);
        Manager.userStopFindMatch(user2, false);
        
        this.user1 = user1;
        this.user2 = user2;
        
        user1.gameConnected = this;
        user2.gameConnected = this;
    }

    public void startGame()
    {
        Id = Manager.getIDGame(this);
        
        user1.Send($"GAME_START {Id} MOVE");
        user2.Send($"GAME_START {Id} WAIT");
        
        Console.WriteLine("Игра стартовала");
    }
    
    public void endGame(User user)
    {
        if (user != user1 && user != user2)
        {
            user.Send($"UNAUTHORIZED {Id}");
            return;
        }
        
        user1.Send($"GAME_END {Id}");
        user2.Send($"GAME_END {Id}");
        
        Console.WriteLine($"Игра #{Id} остановлена ");
        Manager.games.Remove(this);
        
        user1.gameConnected = null;
        user2.gameConnected = null;
    }

    public void moveGame(User user, string id, string s, string x, string y)
    {
        if (user != user1 && user != user2)
        {
            user.Send($"UNAUTHORIZED {Id}");
            return;
        }

        if (user == user1) user2.Send($"GAME_MOVE {Id} {s} {x} {y}");
        else user1.Send($"GAME_MOVE {Id} {s} {x} {y}");
        
    }
}