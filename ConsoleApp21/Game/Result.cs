public static class Result
{
    public static bool Horizontal(Map map)
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

    public static bool Vertical(Map map)
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

    public static bool DiagonalFromUpToDown(Map map)
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
    
    public static bool DiagonalFromDownToUp(Map map)
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

    public static bool AllVariants(Map map, int turn, IPlayer player1, IPlayer player2)
    {
        if (Horizontal(map) || Vertical(map) || DiagonalFromUpToDown(map) || DiagonalFromDownToUp(map))
        {
            if (turn % 2 == 0)
            {
                player1.AddScoreToPlayer();
            }
            else
            {
                player2.AddScoreToPlayer();
            }

            return true;
        }
        return false;
        
    }
}