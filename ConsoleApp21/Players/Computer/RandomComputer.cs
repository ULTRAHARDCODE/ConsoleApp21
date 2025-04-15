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