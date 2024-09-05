using System.Diagnostics;

namespace UsbDeviceReader;

public class ShellRunner
{
    private readonly string _command;
    private readonly string _arguments;
    
    public ShellRunner(string command, string arguments)
    {
        _command = command;
        _arguments = arguments;
    }

    public string Run()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _command,
                Arguments = _arguments,
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