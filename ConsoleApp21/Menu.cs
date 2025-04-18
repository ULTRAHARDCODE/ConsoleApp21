using Newtonsoft.Json;

public static class Menu
{
    
    
}

public static class Users
    {
        public static void CreateUser(List<User> users)
        {
            while (true)
            {
                Console.WriteLine("Enter plyayer's name");
                string name = Console.ReadLine();
                if (users.Any(u => u.name == name))
                {
                    Console.WriteLine("This name is already taken.");
                    continue;
                }

                Console.WriteLine("Change your symbol");
                char symbol;
                while (!char.TryParse(Console.ReadLine(), out symbol))
                    Console.WriteLine("Invalid symbol");
                
                User user = new User(null, symbol, name);
                users.Add(user);
                break;
            }
        }

        public static User GetUser(List<User> users)
        {
            Print(users);
            
            int input;
            while (!int.TryParse(Console.ReadLine(), out input) || input <= 0 || input > users.Count)
                Console.WriteLine("Invalid input");

            return users[input - 1];
        }
        
        public static void Save(List<User> users, string name = "list.txt")
        {
            string directory = Directory.GetCurrentDirectory();
            string path = Path.Combine(directory, name);
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        public static List<User> Load(string name = "list.txt")
        {
            List<User> loadUsers = new List<User>();
            
            string directory = Directory.GetCurrentDirectory();
            string path = Path.Combine(directory, name);
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                loadUsers = JsonConvert.DeserializeObject<List<User>>(json);
            }
            else Console.WriteLine("File isn't found"); 
            
            return loadUsers;
        }

        public static void Print(List<User> users)
        {
            int i = 1;
            foreach (User user in users)
            {
                Console.WriteLine($"{i}. {user.name}");
                i++;
            }
        }
       
        public static class Stats
        {
            public static void Print(List<User> users)
            {
                foreach (var user in users)
                    Console.WriteLine($"Name:{user.name} | Score: {user.score}");

                Console.ReadKey();
            }
        }
    }