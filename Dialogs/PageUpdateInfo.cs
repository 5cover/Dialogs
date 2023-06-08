using Vanara.PInvoke;

namespace Scover.Dialogs;

/// <summary>Page update information.</summary>
public readonly struct PageUpdateInfo
{
    internal PageUpdateInfo(HWND dialog) => Dialog = dialog;

    internal HWND Dialog { get; }
}