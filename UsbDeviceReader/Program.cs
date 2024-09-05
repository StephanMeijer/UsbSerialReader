using System.Reflection;
using System.Text.RegularExpressions;
using UsbDeviceReader;

// In this example, mounted on fs: /dev/cu.usbserial-1140	/dev/cu.usbserial-1170	/dev/tty.usbserial-1140  /dev/tty.usbserial-1170
// In real life, execute:
// var output = new ShellRunner("system_profiler SPUSBDataType").Run();
// In this thing, change the path..
var output = new ShellRunner("cat", "/Users/steve/RiderProjects/UsbDeviceReader/UsbDeviceReader/system_profiler.txt")
    .Run();

var locationIdRegex = @"0x0(.{4}).*"; // PLEASE FIX

// string blockPattern = @"(?<=\n\s+)(.+):\n(?:\s{14}.+\n)+";
Regex blockRegex = new Regex(@"(?<=\n|^)(\s*)(.+):\n(?:\s+.+: .+\n)+", RegexOptions.Multiline);

MatchCollection blockMatches = blockRegex.Matches(output);

List<USBDevice> devices = new List<USBDevice>();


foreach (Match blockMatch in blockMatches)
{
    Dictionary<string, string> propertyMap = new Dictionary<string, string>();

    string[] lines = blockMatch
        .Value
        .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (string line in lines)
    {
        string trimmedLine = line.Trim();

        int colonIndex = trimmedLine.IndexOf(':');
        if (colonIndex > 0) // Ensure there's a key before the colon
        {
            string key = trimmedLine.Substring(0, colonIndex).Trim();

            // First line so assume key is the label :)
            if (!propertyMap.ContainsKey("Label"))
            {
                propertyMap["Label"] = key;
                continue;
            }

            string value = trimmedLine.Substring(colonIndex + 1).Trim();
            propertyMap[key] = value; // Add to dictionary, assuming unique keys

            if (key == "Location ID")
            {
                Match match = Regex.Match(locationIdRegex, value);

                Console.WriteLine(match);

                if (match.Success)
                {
                    Console.WriteLine("Location ID: " + match.Groups["Label"].Value);

                    SerialPath serialPath = new SerialPath(match.Groups[1].Value);

                    if (serialPath.Exists())
                    {
                        propertyMap["Serial Path"] = serialPath.Path;
                    }
                }
            }

            if (key == "Serial Number")
            {
                SerialPath serialPath = new SerialPath(value);

                if (serialPath.Exists())
                {
                    propertyMap["Serial Path"] = serialPath.Path;
                }
            }
        }
    }

    foreach (var pair in propertyMap)
    {
        Console.WriteLine($"{pair.Key}: \"{pair.Value}\"");
    }

    Console.WriteLine("--");
}