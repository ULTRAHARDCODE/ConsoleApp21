namespace ConsoleApp21.Players.User;

class OnlineUserEnemy : IPlayer
{
    Map map;

    public void AddScoreToPlayer()
    {
    }
    
    public bool enemyMove = false;
    public int x,y;
    public OnlineUserEnemy(Map map, Network.Client client)
    {
        this.map = map;
        client.OnMessage += (client, message) =>
        {
            string cmd = message.Split(" ")[0];
            string[] args = message.Split(" ");
            switch (cmd)
            {
                case "GAME_MOVE":
                    x = int.Parse(args[3]);
                    y = int.Parse(args[4]);
                    enemyMove = true;
                    break;
            }
    
        };
    } // Конструктор юзер
    public void Move(Map map)
    {
        map.PrintField();
        while (enemyMove == false)
        {
            
        }
        enemyMove = false;
        map.Move(x, y, new Element('Y'));
    } // Движение на кнопки
}

class OnlineUser : IPlayer
{
    Map map;
    private Network.Client client;

    public void AddScoreToPlayer()
    {
    }
    public OnlineUser(Map map, Network.Client client)
    {
        this.map = map;
        this.client = client;
    } 
    public void Move(Map map)
    { bool turnIsOver = false;
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
                    
                    turnIsOver = map.Move(x, y, new Element('X'));
                    break;
            }
        }

        client.Send($"GAME_MOVE 0 X {x} {y}");
    } // Движение на кнопки
}