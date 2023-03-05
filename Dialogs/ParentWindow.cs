using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace Scover.Dialogs;

/// <summary>A parent window strategy.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ParentWindow
{
    private ParentWindow(HWND hwnd) => Hwnd = (nint)hwnd;

    /// <summary>
    /// The dialog will be modal and the parent window will be the active window attached to the calling
    /// thread's message queue, if it exists. Otherwise the behavior is identical to <see cref="None"/>.
    /// </summary>
    public static ParentWindow Active => new(GetActiveWindow());

    /// <summary>The dialog will be modeless.</summary>
    public static ParentWindow None => new(HWND.NULL);

    internal nint Hwnd { get; }
}