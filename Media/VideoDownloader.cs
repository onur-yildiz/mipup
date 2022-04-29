using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Mipup.Media
{
    internal class VideoDownloader
    {
        public static async Task<string> DownloadVideoAsync(string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streamManifest.GetAudioStreams().GetWithLowestSize();

            var fileNameWithExtension = $"video.{streamInfo.Container}";
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"./media/{fileNameWithExtension}");
            return fileNameWithExtension;
        }
    }
}
