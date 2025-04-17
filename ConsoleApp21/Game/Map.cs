public class Map
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