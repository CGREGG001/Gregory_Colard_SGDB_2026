using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.WriteLine("❌ No commit message file provided");
    Environment.Exit(1);
}

var msg = File.ReadAllText(args[0]).Trim();

var pattern = @"^(feat|fix|chore|docs|refactor|test|style)(\(.+\))?: .+";

if (!Regex.IsMatch(msg, pattern))
{
    Console.WriteLine("❌ Invalid commit message");
    Console.WriteLine("   Expected: type(scope?): description");
    Console.WriteLine("   Example: feat(api): add animal search");
    Environment.Exit(1);
}

Console.WriteLine("✔ Commit message valid");
