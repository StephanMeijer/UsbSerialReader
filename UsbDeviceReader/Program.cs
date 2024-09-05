using System.Reflection;
using System.Text.RegularExpressions;
using UsbDeviceReader;

// In this example, mounted on fs: /dev/cu.usbserial-1140	/dev/cu.usbserial-1170	/dev/tty.usbserial-1140  /dev/tty.usbserial-1170
// In real life, execute:
// var output = new ShellRunner("system_profiler SPUSBDataType").Run();
// In this thing, change the path..
var output = new ShellRunner("cat /Users/steve/RiderProjects/UsbDeviceReader/UsbDeviceReader/system_profiler.txt").Run();

string blockPattern = @"(?<=\n\s+)(.+):\n(?:\s{10,14}.+\n)+";
Regex blockRegex = new Regex(blockPattern, RegexOptions.Multiline);
MatchCollection blockMatches = blockRegex.Matches(output);

List<USBDevice> devices = new List<USBDevice>();

foreach (Match blockMatch in blockMatches)
{
    var firstLineEnd = blockMatch.Value.IndexOf('\n');

    if (firstLineEnd == -1)
    {
        continue;
    }

    string firstLine = blockMatch.Value.Substring(0, firstLineEnd).Trim();

    Dictionary<string, string> propertyMap = new Dictionary<string, string>();

    if (firstLine.EndsWith(":")) // Check if the last character is a colon
    {
        firstLine = firstLine.TrimEnd(':'); // Remove the colon from the end
    }

    propertyMap["Label"] = firstLine;

    string[] otherLines = blockMatch
        .Value
        .Substring(firstLineEnd + 1)
        .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (string line in otherLines)
    {
        string trimmedLine = line.Trim();
        if (string.IsNullOrEmpty(trimmedLine))
        {
            continue; // Skip empty lines
        }

        int colonIndex = trimmedLine.IndexOf(':');
        if (colonIndex > 0) // Ensure there's a key before the colon
        {
            string key = trimmedLine.Substring(0, colonIndex).Trim();
            string value = trimmedLine.Substring(colonIndex + 1).Trim();
            propertyMap[key] = value; // Add to dictionary, assuming unique keys
        }
    }

    foreach (var pair in propertyMap)
    {
        
        Console.WriteLine($"{pair.Key}: {pair.Value}");
    }
    Console.WriteLine("--");
}