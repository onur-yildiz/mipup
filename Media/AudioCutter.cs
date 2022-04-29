using System.Diagnostics;

namespace Mipup.Media
{
    internal class AudioCutter
    {
        public static void Crop(string filePath, string startDuration, int time)
        {
            var cropAudioProcess = new Process();
            cropAudioProcess.StartInfo.FileName = "./ffmpeg.exe";
            cropAudioProcess.StartInfo.Arguments = $"-i {filePath} -ss {startDuration} -t {time} -c:a mp3 ./media/output.mp3";
            cropAudioProcess.StartInfo.RedirectStandardOutput = true;
            if (!cropAudioProcess.Start()) throw new Exception("Could not crop audio.");
            cropAudioProcess.WaitForExit();
        }
    }
}
