namespace URY.BAPS.Client.Cli
{
    public static class CliUtilities
    {
        public static string GetText(string prompt, string defaultValue)
        {
            System.Console.Write($"{prompt} (default: {defaultValue}): ");
            var input = System.Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }
    }
}