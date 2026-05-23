using System.Text;

namespace AnimalShelter.ConsoleApp.UI.Utilities;

public static class UIHelper
{
    public static void ShowHeader()
    {
        // Codes ANSI pour les couleurs
        const string BleuCyan = "\u001b[36;1m";
        const string Bleu = "\u001b[34;1m";
        const string Rouge = "\u001b[31;1m";
        const string GrisFonce = "\u001b[90m";  // Sous-titre fixe
        const string Reset = "\u001b[0m";

        WriteTypewriterLine($"{GrisFonce}  ,-.       _,---._ __{Rouge}   / \\{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce} /  )     .-'       `./{Rouge} /   \\    {BleuCyan}PawShelter Console{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}(  (   ,'            `{Rouge} /    /|   {GrisFonce}Animal Management System{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce} \\  `-\"              \\'{Rouge}\\   / |{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  `.               ,  \\ {Rouge}\\ /  |   {Bleu}Gregory Colard{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}   /`.           ,'-`{Rouge}----Y   |   {Bleu}SGDB Project - ConsoleApp{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  (            ;         {Rouge}|   '   {Bleu}2020-2026{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  |  ,-.    ,-'          {Rouge}|  /{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  |  | (   |  {Rouge}PawShelter | /{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  )  |  \\  `.{Rouge}____________|/{Reset}", 20);
        WriteTypewriterLine($"{GrisFonce}  `--'   `--'{Reset}", 20);

        Console.WriteLine("\n                Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }

    public static void ShowTitleMenu(string title)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        DrawBox(title);
        Console.ResetColor();
    }

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

    public static void DrawBox(string title)
    {
        string line = new string('═', title.Length + 6);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"╔{line}╗");
        Console.WriteLine($"║   {title.ToUpper()}   ║");
        Console.WriteLine($"╚{line}╝");
        Console.ResetColor();
    }

    public static void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[SUCCESS] {msg}");
        Console.ResetColor();
    }

    public static void Warning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n[WARNING] {msg}");
        Console.ResetColor();
    }

    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {msg}");
        Console.ResetColor();
    }
}