using System.Diagnostics;

namespace Mipup.FileSystem
{
    public class FileSetup
    {
        public static void CreateEmptyMediaFolder()
        {
            var resetMediaProcess = new Process();
            resetMediaProcess.StartInfo.FileName = "pwsh.exe";
            resetMediaProcess.StartInfo.Arguments = "/C if (Test-Path -Path './media') {rm -r -Force ./media}; mkdir media";
            resetMediaProcess.StartInfo.RedirectStandardOutput = true;
            if (!resetMediaProcess.Start()) throw new Exception("Could not create media folder.");
            resetMediaProcess.WaitForExit();
        }
    }
}
