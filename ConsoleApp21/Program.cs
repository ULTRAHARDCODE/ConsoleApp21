using ConsoleApp21.Players.User;
using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;
using ConsoleKeyInfo = System.ConsoleKeyInfo;

GameManager gameManager = new GameManager();
gameManager.LoadListWithUsers();


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
    IPlayer chooseUser1;
    IPlayer chooseUser2;

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

        byte input = Convert.ToByte(Console.ReadLine());
        Map map = new Map(size);
        switch (input)
        {
            case 1:
                while (users.Count < 2)
                {
                    AddUserFromApp(map);
                }
                Console.WriteLine("Are you want to add user?");
                ConsoleKeyInfo input2 = Console.ReadKey();
                if (input2.KeyChar == '+')
                {
                    AddUserFromApp(map);
                }
                Console.WriteLine("Choose your name");
                EnterToUser1(EnterInput());
                EnterToUser2(EnterInput());
                StartGame(chooseUser1, chooseUser2, map);
                break;
            case 3:
                ChangeSizeOfField();
                break;
            case 4:
                PrintScoreBar();
                break;
            case 5:
                SaveUsersToFile();
                break;
            // Мой онлайн
            case 7:
                try
                {
                    var client = Network.Client.Connect("62.84.101.222", 8000);
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



    public byte EnterInput()
    {
        byte input;
        int i = 1;
        foreach (User user in users)
        {
            Console.WriteLine($"{i}. {user.name}");
            i++;
        }
        while (!byte.TryParse(Console.ReadLine(), out input) || input <= 0 || input > users.Count)
        {
            Console.WriteLine("Invalid input");
        }
        return (byte)(input - 1);
    }
    public void StartGame(IPlayer player1, IPlayer player2, Map map)
    {

        for (int i = 0; i < map._size * map._size; i++)
        {
            if (turn % 2 == 0)
            {
                player1.Move(map);
            }
            else
            {
                player2.Move(map);
            }
            
            if (Result.AllVariants(map, turn, player1, player2))
            {
                break;
            }
            turn++;
        }




    }  // начало игры, передача хода

    public void AddUserFromApp(Map map)
    {

        bool result = false;
        while (result == false)
        {
            Console.WriteLine("Enter plyayer's name");
            string name = Console.ReadLine();
            Console.WriteLine("Change your symbol");
            char symbol;
            while (!char.TryParse(Console.ReadLine(), out symbol))
            {
                Console.WriteLine("Invalid symbol");
            }
            foreach (var item in users)
            {
                if (users.Any(u => u.name == name))
                {
                    Console.WriteLine("This name is already taken.");
                    break;
                }
                else
                {
                    result = true;
                }
            }

            User user = new User(map, symbol, name);
            users.Add(user);

        }
    }



    public IPlayer EnterToUser1(byte input)
    {
        chooseUser1 = users[input];
        return chooseUser1;
    }
    public IPlayer EnterToUser2(byte input)
    {

        chooseUser2 = users[input];
        return chooseUser2;
    }


    public void SaveUsersToFile(string name = "list.txt")
    {
        string directory = Directory.GetCurrentDirectory();
        string path = Path.Combine(directory, name);
        string json = JsonConvert.SerializeObject(users, Formatting.Indented);
        File.WriteAllText(path, json);
    }
    public void LoadListWithUsers()
    {
        string directory = Directory.GetCurrentDirectory();
        string path = Path.Combine(directory, "list.txt");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            users = JsonConvert.DeserializeObject<List<User>>(json);
        }
        else { Console.WriteLine("File isn't found"); }
    }
    public void PrintScoreBar()
    {

        foreach (var user in users)
        {
            Console.WriteLine($"Name:{user.name}| Score: {user.score}");
        }
        Console.ReadLine();
    }

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








