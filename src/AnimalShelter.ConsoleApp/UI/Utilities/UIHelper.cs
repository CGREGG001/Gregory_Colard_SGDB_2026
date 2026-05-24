using System.Text;

namespace AnimalShelter.ConsoleApp.UI.Utilities;

public static class UIHelper
{
    // ============================================================
    //  COLORS & STYLE
    // ============================================================
    private const ConsoleColor TitleColor = ConsoleColor.Cyan;
    private const ConsoleColor SubtitleColor = ConsoleColor.Yellow;
    private const ConsoleColor SuccessColor = ConsoleColor.Green;
    private const ConsoleColor WarningColor = ConsoleColor.Yellow;
    private const ConsoleColor ErrorColor = ConsoleColor.Red;
    private const ConsoleColor PromptColor = ConsoleColor.DarkGray;

    // ============================================================
    //  HEADER / SPLASH
    // ============================================================
    public static void SplashScreen()
    {
        // Codes ANSI pour les couleurs
        const string BleuCyan = "\u001b[36;1m";
        const string Bleu = "\u001b[34;1m";
        const string Rouge = "\u001b[31;1m";
        const string GrisFonce = "\u001b[90m";  // Sous-titre fixe
        const string Reset = "\u001b[0m";

        string version = "v1.0";

        Console.Clear();
        Console.WriteLine($"\n\n          === SHELTER MANAGEMENT SYSTEM {version} ===\n\n");

        WriteTypewriterLine($"{GrisFonce}  ,-.       _,---._ __{Rouge}   / \\{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce} /  )     .-'       `./{Rouge} /   \\    {BleuCyan}PawShelter Console{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}(  (   ,'            `{Rouge} /    /|   {GrisFonce}Animals Management System{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce} \\  `-\"              \\'{Rouge}\\   / |{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  `.               ,  \\ {Rouge}\\ /  |   {Bleu}Gregory Colard{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}   /`.           ,'-`{Rouge}----Y   |   {Bleu}SGDB Project - ConsoleApp{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  (            ;         {Rouge}|   '   {Bleu}2025-2026{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  |  ,-.    ,-'          {Rouge}|  /{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  |  | (   |  {Rouge}PawShelter | /{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  )  |  \\  `.{Rouge}____________|/{Reset}", 10);
        WriteTypewriterLine($"{GrisFonce}  `--'   `--'{Reset}", 200);

        ShowHeader();

        Console.ResetColor();
        Pause("Press any key to start");
        Console.Clear();
    }

    public static void ShowHeader()
    {
        Console.ForegroundColor = ErrorColor;

        Console.WriteLine("\t╔══════════════════════════════════════════════╗", 2);
        Console.WriteLine("\t║         PAWSHELTER MANAGEMENT SYSTEM         ║", 2);
        Console.WriteLine("\t╚══════════════════════════════════════════════╝", 2);
    }

    // ============================================================
    //  PAUSE
    // ============================================================
    public static void Pause(string message = "Press any key to continue...")
    {
        Console.ForegroundColor = PromptColor;
        Console.WriteLine($"\n\t-> {message}");
        Console.ResetColor();
        Console.ReadKey();
        Console.Clear();
    }

    // ============================================================
    //  TITLES & BOXES
    // ============================================================
    public static void ShowTitle(string title)
    {
        Console.ForegroundColor = TitleColor;
        DrawBox(title);
        Console.ResetColor();
    }

    public static void ShowTitleMenu(string title)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        DrawBox(title);
        Console.ResetColor();
    }
    
    public static void DrawBox(string title)
    {
        string line = new string('═', title.Length + 6);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"╔{line}╗");
        Console.WriteLine($"║   {title.ToUpper()}   ║");
        Console.WriteLine($"╚{line}╝");
        Console.ResetColor();
    }

    // ============================================================
    //  TYPEWRITER EFFECT
    // ============================================================
    public static void WriteTypewriterLine(string line, int delayMs = 5)
    {
        int i = 0;
        while (i < line.Length)
        {
            // Détection et application instantanée des codes ANSI (ex: \u001b[31;1m)
            if (line[i] == '\u001b')
            {
                StringBuilder ansiCode = new StringBuilder();
                while (i < line.Length && line[i] != 'm')
                {
                    ansiCode.Append(line[i]);
                    i++;
                }
                if (i < line.Length && line[i] == 'm')
                {
                    ansiCode.Append('m');
                    i++;
                }
                // On applique le code couleur d'un coup dans le terminal sans délai
                Console.Write(ansiCode.ToString());
                continue;
            }

            // Si c'est un caractère normal, on l'affiche et on attend un peu
            Console.Write(line[i]);
            Thread.Sleep(delayMs);
            i++;
        }
        Console.WriteLine(); // Fin de ligne
    }

    // ============================================================
    //  MESSAGES
    // ============================================================
    public static void Success(string msg)
    {
        Console.ForegroundColor = SuccessColor;
        Console.WriteLine($"\n✔ [SUCCESS] {msg}");
        Console.ResetColor();
    }

    public static void Warning(string msg)
    {
        Console.ForegroundColor = WarningColor;
        Console.WriteLine($"\n⚠ [WARNING] {msg}");
        Console.ResetColor();
    }

    public static void Error(string msg)
    {
        Console.ForegroundColor = ErrorColor;
        Console.WriteLine($"\n✖ [ERROR] {msg}");
        Console.ResetColor();
    }

    // ============================================================
    //  INPUT HELPERS
    // ============================================================
    public static Guid AskGuid(string label)
    {
        Guid value;
        do
        {
            Console.ForegroundColor = PromptColor;
            Console.Write($"{label}: ");
            Console.ResetColor();
        }
        while (!Guid.TryParse(Console.ReadLine(), out value));

        return value;
    }

    public static DateTime AskDate(string label, bool optional = false)
    {
        while (true)
        {
            Console.ForegroundColor = PromptColor;
            Console.Write($"{label} (yyyy-mm-dd){(optional ? " or empty" : "")}: ");
            Console.ResetColor();

            string? input = Console.ReadLine();

            if (optional && string.IsNullOrWhiteSpace(input))
                return DateTime.Now;

            if (DateTime.TryParse(input, out DateTime date))
                return date;

            Warning("Invalid date format.");
        }
    }

    public static T AskEnum<T>(string label) where T : struct, Enum
    {
        Console.WriteLine($"\n{label}:");

        var values = Enum.GetValues<T>();
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($" {i + 1}. {values[i]}");

        while (true)
        {
            Console.ForegroundColor = PromptColor;
            Console.Write("Select option: ");
            Console.ResetColor();

            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 1 && choice <= values.Length)
                return values[choice - 1];

            Warning("Invalid selection.");
        }
    }

    public static bool Confirm(string message)
    {
        Console.ForegroundColor = PromptColor;
        Console.Write($"{message} (y/n): ");
        Console.ResetColor();

        string? input = Console.ReadLine()?.ToLower();
        return input == "y" || input == "yes";
    }

    // ============================================================
    //  TABLE RENDERER
    // ============================================================
    public static void ShowTable(string[] headers, List<string[]> rows)
    {
        int[] widths = new int[headers.Length];

        for (int i = 0; i < headers.Length; i++)
            widths[i] = headers[i].Length;

        foreach (var row in rows)
            for (int i = 0; i < row.Length; i++)
                widths[i] = Math.Max(widths[i], row[i].Length);

        // Header
        Console.ForegroundColor = SubtitleColor;
        for (int i = 0; i < headers.Length; i++)
            Console.Write($"{headers[i].PadRight(widths[i] + 2)}");
        Console.ResetColor();
        Console.WriteLine();

        // Separator
        Console.WriteLine(new string('-', widths.Sum() + (2 * widths.Length)));

        // Rows
        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length; i++)
                Console.Write($"{row[i].PadRight(widths[i] + 2)}");
            Console.WriteLine();
        }
    }

    public static void LoadingDots(string message, int dotCount = 3, int delay = 350)
    {
        Console.Write($"\n{message}");
        for (int i = 0; i < dotCount; i++)
        {
            Thread.Sleep(delay);
            Console.Write(".");
        }
        Console.WriteLine();
    }
}
