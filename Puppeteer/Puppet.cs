using PuppeteerSharp;
using PuppeteerSharp.Input;
using Mipup.FileSystem;
using Mipup.Media;
using Mipup.ConsoleRW;

namespace Mipup
{
    internal class Puppet
    {
        public static async Task<string> CutAndUploadAudio(string name, string url, string startDuration, int time)
        {
            Browser? browser = null;
            try
            {
                FileSetup.CreateEmptyMediaFolder();
                var fileName = await VideoDownloader.DownloadVideoAsync(url);
                var filePath = $"./media/{fileName}";
                AudioCutter.Crop(filePath, startDuration, time);

                Console.WriteLine("Downloading chromium...");
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                Console.Clear();
                browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });

                var page = await browser.NewPageAsync();
                var client = await page.Target.CreateCDPSessionAsync();
                var mouse = new Mouse(client, page.Keyboard);

                Console.Clear();
                await Login(page);
                await Upload(page, name);
                return await ExtractURL(page);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Could not extract URL!.";
            }
            finally
            {
                if (browser != null) await browser.CloseAsync();
            }
        }

        static async Task Login(Page page)
        {
            Console.WriteLine("Logging into myinstants...");
            var creds = LoginPrompt.GetCredentials();
            await page.GoToAsync("https://www.myinstants.com/accounts/login/?next=/new/");
            await page.TypeAsync("input[name=login]", creds[0]);
            await page.TypeAsync("input[name=password]", creds[1]);
            await page.ClickAsync("button[type=submit]");

            //await page.WaitForNavigationAsync();
            await page.WaitForSelectorAsync("#id_name");
            Console.WriteLine("Logged in to myinstants.");
        }

        static async Task Upload(Page page, string name)
        {
            Console.WriteLine("Uploading audio...");
            await page.TypeAsync("#id_name", name + Guid.NewGuid().ToString());
            var fileChooserDialogTask = page.WaitForFileChooserAsync();
            await Task.WhenAll(fileChooserDialogTask, page.ClickAsync("input[name=sound]"));
            var fileChooser = await fileChooserDialogTask;
            await fileChooser.AcceptAsync("./media/output.mp3");
            var termsCheckbox = await page.WaitForSelectorAsync("input[type=checkbox]");
            await page.EvaluateFunctionAsync("cb => cb.click()", termsCheckbox);
            await page.FocusAsync("button[type=submit]");
            await page.Keyboard.PressAsync("Enter");

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

            var url = await page.EvaluateFunctionAsync<string?>(@"
                () => {
                    return document.querySelector('a[download]').href.toString(); 
                }
            ");

            return url ?? "Could not extract URL!.";
        }
    }
}
