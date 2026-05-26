namespace AnimalShelter.ConsoleApp.UI.Utilities
{
    public static class ConsoleHelper
    {
        public static string GetRequiredString(string prompt)
        {
            string? input;
            do
            {
                Console.Write($"{prompt}: ");
                input = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static int GetInt(string prompt)
        {
            int value;
            Console.Write($"{prompt}: ");

            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Invalid number. Try again: ");
            }

            return value;
        }

        public static string GetString(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        public static bool GetBool(string prompt)
        {
            Console.Write($"{prompt} (y/n): ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            return input == "y" || input == "yes" || input == "o";
        }

        public static DateTime? GetOptionalDate(string prompt)
        {
            Console.Write($"{prompt} (yyyy-mm-dd) [Enter to skip]: ");

            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (DateTime.TryParse(input, out DateTime result))
            {
                return result;
            }

            Console.WriteLine("Invalid date format. Skipping.");

            return null;
        }

        public static T GetEnum<T>(string prompt) where T : struct, Enum
        {
            var values = Enum.GetValues<T>();

            while (true)
            {
                Console.WriteLine($"{prompt}:");

                for (int i = 0; i < values.Length; i++)
                {
                    Console.WriteLine($"{i}. {values[i]}");
                }

                Console.Write("Choice: ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 0 && choice < values.Length)
                {
                    return values[choice];
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        public static string GetStringWithDefault(string prompt, string defaultValue)
        {
            //Console.ForegroundColor = PromptColor;
            Console.Write($"{prompt} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{defaultValue}]");
            //Console.ForegroundColor = PromptColor;
            Console.Write(": ");
            Console.ResetColor();

            string? input = Console.ReadLine();

            // Si l'utilisateur appuie sur Entrée sans rien taper, on retourne la valeur actuelle
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }
    }
}
