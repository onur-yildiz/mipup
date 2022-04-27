using System.Diagnostics;
using System.Security.Cryptography;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using CommandLine;
using Mipup.Models.Arguments;

namespace Mipup
{
    public class AudioCutter
    {
        static async Task Main(string[] args)
        {
            try
            {
                var options = Parser.Default.ParseArguments<Options>(args).MapResult(o => o, (e) => { throw new Exception(e.First().ToString()); });
                await CutAndUploadAudio(options.Name, options.Url, options.StartDuration, options.Time);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static async Task<string?> CutAndUploadAudio(string name, string url, string startDuration, int time)
        {
            CreateEmptyMediaFolder();
            var fileName = await DownloadVideoAsync(url);
            CropAudio(fileName, startDuration, time);

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            try
            {
                var page = await browser.NewPageAsync();
                var client = await page.Target.CreateCDPSessionAsync();
                var mouse = new Mouse(client, page.Keyboard);

                await Login(page);
                await Upload(page, name);
                var audioURL = await ExtractURL(page) ?? "Could not extract URL!.";

                Console.Write("\nAudio URL: ");
                var defaultConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(audioURL);
                Console.ForegroundColor = defaultConsoleColor;
                return audioURL;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }


        static void CreateEmptyMediaFolder()
        {
            var resetMediaProcess = new Process();
            resetMediaProcess.StartInfo.FileName = "pwsh.exe";
            resetMediaProcess.StartInfo.Arguments = "/C if (Test-Path -Path './media') {rm -r -Force ./media}; mkdir media";
            resetMediaProcess.StartInfo.RedirectStandardOutput = true;
            if (!resetMediaProcess.Start()) throw new Exception("Could not create media folder.");
            resetMediaProcess.WaitForExit();
        }

        static async Task<string> DownloadVideoAsync(string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streamManifest.GetAudioStreams().GetWithLowestSize();

            var videoName = $"video.{streamInfo.Container}";
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"./media/{videoName}");
            return videoName;
        }

        static void CropAudio(string videoName, string startDuration, int time)
        {
            var cropAudioProcess = new Process();
            cropAudioProcess.StartInfo.FileName = "./ffmpeg.exe";
            cropAudioProcess.StartInfo.Arguments = $"-i ./media/{videoName} -ss {startDuration} -t {time} -c:a mp3 ./media/output.mp3";
            cropAudioProcess.StartInfo.RedirectStandardOutput = true;
            if (!cropAudioProcess.Start()) throw new Exception("Could not crop audio.");
            cropAudioProcess.WaitForExit();
        }

        static async Task Login(Page page)
        {
            Console.Clear();
            Console.WriteLine("Logging in to myinstants...");
            var creds = GetCredentials();
            await page.GoToAsync("https://www.myinstants.com/accounts/login/?next=/new/");
            await page.TypeAsync("input[name=login]", creds[0]);
            await page.TypeAsync("input[name=password]", creds[1]);
            await page.ClickAsync("div.input-field>button[type=submit]");

            //await page.WaitForNavigationAsync();
            await page.WaitForSelectorAsync("#id_name");
            Console.WriteLine("Logged in to myinstants.");
        }

        static async Task Upload(Page page, string name)
        {
            Console.WriteLine("Uploading audio...");
            await page.TypeAsync("#id_name", name + Guid.NewGuid().ToString());
            var fileChooserDialogTask = page.WaitForFileChooserAsync();
            var termsCheckbox = await page.WaitForSelectorAsync("input[type=checkbox]");
            await Task.WhenAll(fileChooserDialogTask, page.ClickAsync("input[name=sound]"));
            var fileChooser = await fileChooserDialogTask;
            await fileChooser.AcceptAsync("./media/output.mp3");
            await page.EvaluateFunctionAsync("cb => cb.click()", termsCheckbox);
            await page.ClickAsync("input[type=submit]");

            //await page.WaitForNavigationAsync();
            await page.WaitForSelectorAsync("a.instant-link");
            Console.WriteLine("Uploaded Audio.");
        }

        static async Task<string> ExtractURL(Page page)
        {
            Console.WriteLine("Navigating to the audio page...");
            await page.EvaluateFunctionAsync(@"
            () => {
                const audioList = document.querySelectorAll('a.instant-link'); 
                audioList[audioList.length -1].click();
            }
        ");

            //await page.WaitForNavigationAsync();
            await page.WaitForSelectorAsync("a[download]");
            Console.WriteLine("Navigated to audio page.\nExtracting URL...");

            return await page.EvaluateFunctionAsync<string>(@"
            () => {
                return document.querySelector('a[download]').href.toString(); 
            }
        ");
        }

        static string[] GetCredentials()
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
            } while (key != ConsoleKey.Enter && password.Length > 0);
            Console.Write("\nWould you like to save your credentials for the future? (YOUR CREDENTIALS WILL BE STORED AS PLAIN TEXT) [y/N]: ");

            if (new string[] { "y", "Y" }.Contains(Console.ReadLine()))
            {
                File.WriteAllText("./mipupc", $"{userId}||{password}");
            }

            return new string[] {userId, password };
        }
    }
}