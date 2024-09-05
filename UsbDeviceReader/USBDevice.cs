namespace UsbDeviceReader;

public record USBDevice(
    string? Label,
    string? ProductId,
    string? VendorId,
    string? VendorLabel,
    string? Version,
    string? SerialNumber,
    string? LocationId,
    string? Manufacturer,
    string? SerialPath
);