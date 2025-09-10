using System.Diagnostics;

namespace WebLiraLauncher;

public class DotNetChecker
{
    public static bool IsDotNet8Installed()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process();
            process.StartInfo = processInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string firstSymbol = output[0].ToString();
            int dotnetVersion = int.Parse(firstSymbol);

            if (dotnetVersion >= 8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }
}
