using Vanara.PInvoke;

namespace Scover.Dialogs;

/// <summary>ID control update information.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public readonly struct IdControlUpdateInfo
{
    internal IdControlUpdateInfo(HWND dialog, int controlId) => (Dialog, ControlId) = (dialog, controlId);

    internal int ControlId { get; }
    internal HWND Dialog { get; }
}
