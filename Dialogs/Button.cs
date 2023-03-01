using System.Diagnostics;

using Vanara.InteropServices;

using static Vanara.PInvoke.User32.MB_RESULT;

namespace Scover.Dialogs;

/// <summary>A dialog button with text. Can represent a push button or a command link.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(_nativeText)}}}")]
public sealed class Button : ButtonBase, IEquatable<Button?>, ITextControl, IDisposable
{
    private readonly SafeLPWSTR _nativeText = SafeLPWSTR.Null;

    /// <param name="text">The push button or command link text.</param>
    /// <param name="note">The command link supplemental instruction.</param>
    public Button(string text, string? note = null) => _nativeText = new(note is null ? text : $"{text}\n{note}");

    /// <summary>Gets the <i>Abort</i> button.</summary>
    public static CommonButton Abort { get; } = new(1 << 16, IDABORT);

    /// <summary>Gets the <i>Cancel</i> button.</summary>
    /// <remarks>
    /// Similarly to <see cref="Page.IsCancelable"/>, adding this button to <see cref="Page.Buttons"/>
    /// causes the dialog window to respond to typical cancel actions (Alt-F4 and Escape) and have a close
    /// button on its title bar.
    /// </remarks>
    public static CommonButton Cancel { get; } = new(TDCBF_CANCEL_BUTTON, IDCANCEL);

    /// <summary>Gets the <i>Close</i> button.</summary>
    public static CommonButton Close { get; } = new(TDCBF_CLOSE_BUTTON, 8);

    /// <summary>Gets the <i>Continue</i> button.</summary>
    public static CommonButton Continue { get; } = new(1 << 19, IDCONTINUE);

    /// <summary>Gets the <i>Help</i> button.</summary>
    /// <remarks>
    /// This button is non-committing and raises the <see cref="Page.HelpRequested"/> event when clicked.
    /// </remarks>
    public static CommonButton Help { get; } = new(1 << 20, 9);

    /// <summary>Gets the <i>Ignore</i> button.</summary>
    public static CommonButton Ignore { get; } = new(1 << 17, IDIGNORE);

    /// <summary>Gets the <i>No</i> button.</summary>
    public static CommonButton No { get; } = new(TDCBF_NO_BUTTON, IDNO);

    /// <summary>Gets the <i>OK</i> button.</summary>
    public static CommonButton OK { get; } = new(TDCBF_OK_BUTTON, IDOK);

    /// <summary>Gets the <i>Retry</i> button.</summary>
    public static CommonButton Retry { get; } = new(TDCBF_RETRY_BUTTON, IDRETRY);

    /// <summary>Gets the <i>Try Again</i> button.</summary>
    public static CommonButton TryAgain { get; } = new(1 << 18, IDTRYAGAIN);

    /// <summary>Gets the <i>Yes</i> button.</summary>
    public static CommonButton Yes { get; } = new(TDCBF_YES_BUTTON, IDYES);

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <inheritdoc/>
    public void Dispose()
    {
        _nativeText.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Button);

    /// <inheritdoc/>
    public bool Equals(Button? other) => other is not null && _nativeText.Equals(other._nativeText);

    /// <inheritdoc/>
    public override int GetHashCode() => _nativeText.GetHashCode();
}