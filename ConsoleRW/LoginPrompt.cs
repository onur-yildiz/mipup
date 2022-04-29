namespace Mipup.ConsoleRW
{
    internal class LoginPrompt
    {
        public static string[] GetCredentials()
        {
            string[] creds;
            if (!File.Exists("./mipupc")) return PromptCredentials();
            creds = File.ReadAllLines("./mipupc").First().Split("||");
            if (creds.Length != 2) return PromptCredentials();
            return creds;
        }

        static string[] PromptCredentials()
        {
            string? userId;
            string? password = string.Empty;
            do
            {
                Console.Clear();
                Console.Write("User ID: ");
                userId = Console.ReadLine();

            } while (String.IsNullOrEmpty(userId));

            ConsoleKey key;
            Console.Write("Password: ");
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter || password.Length == 0);
            Console.Write("\nWould you like to save your credentials for the future? (YOUR CREDENTIALS WILL BE STORED AS PLAIN TEXT) [y/N]: ");

            if (new string[] { "y", "Y" }.Contains(Console.ReadLine()))
            {
                File.WriteAllText("./mipupc", $"{userId}||{password}");
            }

            return new string[] { userId, password };
        }
    }
}
