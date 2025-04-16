var client = Network.Client.Connect("127.0.0.1", 8000);
client.OnMessage += (client, message) =>
{
    string cmd = message.Split(" ")[0];
    string[] args = message.Split(" ");
    switch (cmd)
    {
        case "FIND_MATCHES_START":
            break;
        case "FIND_MATCHES_STOP":
            break;
        case "FIND_MATCH_PLAYERS":
            Console.WriteLine($"Игроков в поиске: {args[1]}");
            break;
        case "GAME_START":
            Console.WriteLine("Игра началась!");
            
            if (args[2] == "MOVE") Console.WriteLine("Ваш ход!");
            else Console.WriteLine("Ход соперника!");
            break;
        case "GAME_END":
            Console.WriteLine("Игра остановлена!");
            break;
        case "GAME_MOVE":
            Console.WriteLine($"Соперник походил x:{args[3]} y:{args[4]} ");
            break;
        
    }
    
};