namespace Scover.Dialogs;

internal static class WindowsVersion
{
    private static readonly Version eight = new(6, 2, 0), taskDialogMinVersion = new(6, 0, 5243), xp = new(5, 1, 2600);
    public static bool IsWindows8OrLater => OS.Platform == PlatformID.Win32NT && OS.Version >= eight;
    public static bool IsWindowsXPOrLater => OS.Platform == PlatformID.Win32NT && OS.Version >= xp;
    public static bool SupportsTaskDialogs => OS.Platform == PlatformID.Win32NT && OS.Version >= taskDialogMinVersion;
    private static OperatingSystem OS => Environment.OSVersion;
}