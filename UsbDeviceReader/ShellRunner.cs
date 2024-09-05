using System.Diagnostics;

namespace UsbDeviceReader;

public class ShellRunner
{
    private readonly string _command;
    
    public ShellRunner(string command)
    {
        _command = command;
    }

    public string Run()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{_command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}