using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

namespace Scover.Dialogs;

/// <summary>A dialog push button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
public sealed class Button : TextCommitControl
{
    /// <param name="text">The text of the push button.</param>
    public Button(string text) : base(text) => Text = text;

    /// <summary>Gets the <i>Abort</i> button.</summary>
    public static CommonButton Abort { get; } = new(1 << 16, MB_RESULT.IDABORT);

    /// <summary>Gets the <i>Close</i> button.</summary>
    public static CommonButton Close { get; } = new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON, 8);

    /// <summary>Gets the <i>Continue</i> button.</summary>
    public static CommonButton Continue { get; } = new(1 << 19, MB_RESULT.IDCONTINUE);

    /// <summary>Gets the <i>Help</i> button.</summary>
    /// <remarks>
    /// This button is non-committing and raises the <see cref="Page.HelpRequested"/> event when clicked.
    /// </remarks>
    public static CommonButton Help { get; } = new(1 << 20, 9);

    /// <summary>Gets the <i>Ignore</i> button.</summary>
    public static CommonButton Ignore { get; } = new(1 << 17, MB_RESULT.IDIGNORE);

    /// <summary>Gets the <i>No</i> button.</summary>
    public static CommonButton No { get; } = new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_NO_BUTTON, MB_RESULT.IDNO);

    /// <summary>Gets the <i>OK</i> button.</summary>
    public static CommonButton OK { get; } = new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON, MB_RESULT.IDOK);

    /// <summary>Gets the <i>Retry</i> button.</summary>
    public static CommonButton Retry { get; } = new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_RETRY_BUTTON, MB_RESULT.IDRETRY);

    /// <summary>Gets the <i>Try Again</i> button.</summary>
    public static CommonButton TryAgain { get; } = new(1 << 18, MB_RESULT.IDTRYAGAIN);

    /// <summary>Gets the <i>Yes</i> button.</summary>
    public static CommonButton Yes { get; } = new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_YES_BUTTON, MB_RESULT.IDYES);

    /// <summary>Gets the text of this push button.</summary>
    public string Text { get; }
}