namespace YoutubeExplode.Videos.Streams
{
    public static class StreamInfoExtensions
    {
        //
        // Summary:
        //     Gets the stream with the lowest size.
        public static IStreamInfo GetWithLowestSize(this IEnumerable<IStreamInfo> streamInfos) 
        {
            return streamInfos.OrderBy((IStreamInfo s) => s.Size).First();
        }
    }
}
