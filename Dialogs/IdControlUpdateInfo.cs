using Vanara.PInvoke;

namespace Scover.Dialogs;

/// <summary>ID control update information.</summary>
public readonly struct IdControlUpdateInfo
{
    internal IdControlUpdateInfo(HWND dialog, int controlId) => (Dialog, ControlId) = (dialog, controlId);

    internal int ControlId { get; }
    internal HWND Dialog { get; }
}