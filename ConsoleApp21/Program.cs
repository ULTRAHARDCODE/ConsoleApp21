using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;
GameManager gameManager = new GameManager();
gameManager.LoadListWithUsers();


while (true)
{
    gameManager.BeginingTheGame();
}


class Map
{
    public int _size;
    public Element[,] Field;
    public Map(int size)
    {
        Field = new Element[size, size];

        _size = size;
    } // конструктор

    public bool Move(int x, int y, Element icon)
    {

        if (x < 0 || y < 0 || x >= _size || y >= _size)
        {
            return false;
        }
        if (Field[x, y] != null)
        {
            return false;
        }


        Field[x, y] = icon;
        return true;
    } //постановка символа

    public void PrintMapWithCoursor(int x, int y)
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (x == i && y == j)
                {
                    if (Field[i, j] == null)
                        Console.Write("[]");
                    else
                        Console.Write($"[{Field[i, j].Icon}]");
                }
                else
                {
                    if (Field[i, j] == null)
                        Console.Write("- ");
                    else
                        Console.Write(Field[i, j].Icon + " ");
                }
            }
            Console.WriteLine();
        }

    }

    public void PrintField()
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (Field[i, j] == null)
                {
                    Console.Write("- ");
                }
                else
                {
                    Console.Write(Field[i, j].Icon + " ");
                }
            }
            Console.WriteLine();
        }
    } // отрисовка поля
}

class Element
{
    public char Icon;

    public Element(char icon)
    {
        Icon = icon;
    } // конструктор иконки
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
                            "\n 6. Exit");

        byte input = Convert.ToByte(Console.ReadLine());
        switch (input)
        {
            case 1:
                Map map = new Map(size);
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

            
            if (CheckWin(map))
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



    private bool CheckWinHorisontal(Map map)
    {

        for (int i = 0; i < map._size; i++)
        {
            for (int j = 0; j < map._size - 2; j++)
            {


                if (map.Field[i, j] == null || map.Field[i, j + 1] == null || map.Field[i, j + 2] == null) continue;
                if (map.Field[i, j].Icon == map.Field[i, j + 1].Icon && map.Field[i, j].Icon == map.Field[i, j + 2].Icon)
                {
                    Console.WriteLine($"Win {map.Field[i, j].Icon}");
                    return true;
                }

            }
        }
        return false;


    }
    private bool CheckWinVertical(Map map)
    {

        for (int i = 0; i < map._size - 2; i++)
        {
            for (int j = 0; j < map._size; j++)
            {
                if (map.Field[i, j] == null || map.Field[i + 1, j] == null || map.Field[i + 2, j] == null) continue;

                if (map.Field[i, j].Icon == map.Field[i + 1, j].Icon && map.Field[i, j].Icon == map.Field[i + 2, j].Icon)
                {
                    Console.WriteLine($"Win {map.Field[i, j].Icon}");
                    return true;

                }

            }

        }
        return false;


    }
    private bool CheckWinDiagonalFromUpToDown(Map map)
    {
        try
        {
            for (int i = 0; i < map._size - 2; i++)
            {
                for (int j = 0; j < map._size - 2; j++)
                {
                    if (map.Field[i, j] == null || map.Field[i + 1, j + 1] == null || map.Field[i + 2, j + 2] == null) continue;
                    if (map.Field[i, j].Icon == map.Field[i + 1, j + 1].Icon && map.Field[i, j].Icon == map.Field[i + 2, j + 2].Icon)
                    {
                        Console.WriteLine($"Win {map.Field[i, j].Icon}");
                        return true;
                    }
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    private bool CheckWinDiagonalFromDownToUp(Map map)
    {

        for (int i = 2; i < map._size; i++)
        {
            for (int j = 0; j < map._size - 2; j++)
            {

                if (map.Field[i, j] == null || map.Field[i - 1, j + 1] == null || map.Field[i - 2, j + 2] == null) continue;
                if (map.Field[i, j].Icon == map.Field[i - 1, j + 1].Icon && map.Field[i, j].Icon == map.Field[i - 2, j + 2].Icon)
                {
                    Console.WriteLine($"Win {map.Field[i, j].Icon}");
                    return true;
                }
            }
        }
        return false;


    }
    private bool CheckWin(Map map)
    {

        if (CheckWinHorisontal(map) || CheckWinVertical(map) || CheckWinDiagonalFromUpToDown(map) || CheckWinDiagonalFromDownToUp(map))
        {
            if (turn % 2 == 0)
            {
                chooseUser1.AddScoreToPlayer();
            }
            else
            {
                chooseUser2.AddScoreToPlayer();
            }

            return true;
        }
        return false;

    } // проверка на выйгрыш




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



interface IPlayer
{
    public void Move(Map map);
    public void AddScoreToPlayer();


}

class Computer : IPlayer
{
    char icon;
    Map map;
    public Computer(Map map, char icon)
    {
        this.map = map;
        this.icon = icon;
    }
    public void AddScoreToPlayer()
    {

    }
    public void Move(Map map)
    {

        Random random = new Random();
        int x = random.Next(0, map.Field.GetLength(0));
        int y = random.Next(0, map.Field.GetLength(1));

        while (map.Move(x, y, new Element(icon)) == false)
        {
            x = random.Next(0, map.Field.GetLength(0));
            y = random.Next(0, map.Field.GetLength(1));
        }


    }

}

class User : IPlayer
{
    Map map;
    public int score;
    public string name;
    public char icon;

    public void AddScoreToPlayer()
    {
        score++;
    }
    public User(Map map, char icon, string name)
    {

        this.map = map;
        this.icon = icon;
        this.name = name;
    } // Конструктор юзер
    public void Move(Map map)
    {
        bool turnIsOver = false;
        int x = 0;
        int y = 0;


        while (turnIsOver == false)
        {
            map.PrintMapWithCoursor(x, y);
            ConsoleKeyInfo input = Console.ReadKey();
            ConsoleKey button = input.Key;
            Console.Clear();
            switch (button)
            {

                case ConsoleKey.UpArrow:
                    if (x > 0)
                        x--;
                    break;
                case ConsoleKey.DownArrow:
                    if (x < map._size - 1)
                        x++;
                    break;
                case ConsoleKey.RightArrow:
                    if (y < map._size - 1)
                        y++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (y > 0)
                        y--;
                    break;
                default:


                    turnIsOver = map.Move(x, y, new Element(icon));
                    break;
            }
        }
    } // Движение на кнопки
}


enum MyAction
{
    PvP = 1,
    PvE = 2,
    ChangeSize = 3,
    CheckMyScore = 4,
    SaveScore = 5,
    Exit = 6
}