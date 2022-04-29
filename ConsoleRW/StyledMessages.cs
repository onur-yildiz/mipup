namespace Mipup.ConsoleRW
{
    internal class StyledMessages
    {
        public static void PrintAudioUrl(string url)
        {
            Console.Write("\nAudio URL: ");
            var defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(url);
            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}
