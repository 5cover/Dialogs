namespace Scover.Dialogs;

internal static class WindowsVersion
{
    private static readonly OperatingSystem os = Environment.OSVersion;
    private static readonly Version taskDialogMinVersion = new(6, 0, 5243);
    private static readonly Version xp = new(5, 1, 2600);

    public static bool IsWindowsXPOrLater => os.Platform == PlatformID.Win32NT && os.Version >= xp;

    public static bool SupportsTaskDialogs => os.Platform == PlatformID.Win32NT && os.Version >= taskDialogMinVersion;
}