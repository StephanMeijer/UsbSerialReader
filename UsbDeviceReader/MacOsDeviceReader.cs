using System.Text.RegularExpressions;

namespace UsbDeviceReader;

public class MacOsDeviceReader : IDeviceReader
{
    public static void List()
    {
        try
        {
            // List all serial devices under /dev/cu.*
            string[] serialDevices = Directory.GetFiles("/dev/", "cu.*");

            // Get details of all connected USB devices via 'ioreg'
            string ioregOutput = new ShellRunner("ioreg -p IOUSB -l").Run();

            // Split the output into blocks for each device
            string[] deviceBlocks = Regex.Split(ioregOutput, @"\+-o\s");

            foreach (string device in serialDevices)
            {
                Console.WriteLine($"Serial Device: {device}");

                // Try to match device with vendor ID and serial number
                var vendorInfo = GetVendorAndSerial(deviceBlocks, device);
                if (vendorInfo != null)
                {
                    Console.WriteLine($"  Vendor ID: {vendorInfo.Item1}");
                    Console.WriteLine($"  Serial Number: {vendorInfo.Item2}");
                }
                else
                {
                    Console.WriteLine("  No vendor info found.");
                }

                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static Tuple<string, string> GetVendorAndSerial(string[] deviceBlocks, string serialDevice)
    {
        foreach (var block in deviceBlocks)
        {
            // Check if the current block is related to the serial device in question
            if (block.Contains(serialDevice))
            {
                // Use regex to extract the Vendor ID and Serial Number
                Regex idVendorRegex = new Regex("\"idVendor\"\\s*=\\s*(\\d+)");
                Regex serialNumberRegex = new Regex("\"USB Serial Number\"\\s*=\\s*\"([^\"]+)\"");

                var vendorMatch = idVendorRegex.Match(block);
                var serialMatch = serialNumberRegex.Match(block);

                if (vendorMatch.Success && serialMatch.Success)
                {
                    return new Tuple<string, string>(vendorMatch.Groups[1].Value, serialMatch.Groups[1].Value);
                }
            }
        }

        return null;
    }
}