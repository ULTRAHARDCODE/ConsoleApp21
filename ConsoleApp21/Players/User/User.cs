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