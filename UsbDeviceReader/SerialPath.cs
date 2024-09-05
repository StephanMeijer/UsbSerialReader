namespace UsbDeviceReader;

public class SerialPath
{
    public string Path { get; private set; }
    
    public SerialPath(string identifier)
    {
        Path = "/dev/tty.usb-serial-" + identifier;
    }

    public Boolean Exists()
    {
        return File.Exists(Path);
    }

    public string? PathIfExists()
    {
        return Exists() ? Path : null;
    }
}