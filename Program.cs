using CommandLine;
using Mipup.Models.Arguments;
using Mipup.ConsoleRW;

namespace Mipup
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var options = Parser.Default.ParseArguments<Options>(args).MapResult(o => o, (e) => { throw new Exception(e.First().ToString()); });
                var url = await Puppet.CutAndUploadAudio(options.Name, options.Url, options.StartDuration, options.Time);
                StyledMessages.PrintAudioUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}