using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog push or command link button This class cannot be inherited.</summary>
public sealed class Button : CommitControl, INativeProvider<string>
{
    private readonly string _text;

    /// <summary>Initializes the instance of the <see cref="Button"/> class.</summary>
    /// <param name="label">The label of the commit control</param>
    /// <param name="note">The supplemental instruction of the commit control</param>
    public Button(string label, string? note = null) => _text = note is null ? label : $"{label}\n{note}";

    /// <summary>Gets a new <i>Abort</i> button.</summary>
    public static CommonButton Abort => new(1 << 16, MB_RESULT.IDABORT);

    /// <summary>Gets a new <i>Cancel</i> button.</summary>
    public static CommonButton Cancel => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON, MB_RESULT.IDCANCEL);

    /// <summary>Gets a new <i>Close</i> button.</summary>
    public static CommonButton Close => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON, 8);

    /// <summary>Gets a new <i>Continue</i> button.</summary>
    public static CommonButton Continue => new(1 << 19, MB_RESULT.IDCONTINUE);

    /// <summary>Gets a new <i>Help</i> button.</summary>
    /// <remarks>This button is non-committing and raises the <see cref="Page.HelpRequested"/> event when clicked.</remarks>
    public static CommonButton Help => new(1 << 20, 9);

    /// <summary>Gets a new <i>Ignore</i> button.</summary>
    public static CommonButton Ignore => new(1 << 17, MB_RESULT.IDIGNORE);

    /// <summary>Gets a new <i>No</i> button.</summary>
    public static CommonButton No => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_NO_BUTTON, MB_RESULT.IDNO);

    /// <summary>Gets a new <i>OK</i> button.</summary>
    public static CommonButton OK => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON, MB_RESULT.IDOK);

    /// <summary>Gets a new <i>Retry</i> button.</summary>
    public static CommonButton Retry => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_RETRY_BUTTON, MB_RESULT.IDRETRY);

    /// <summary>Gets a new <i>Try Again</i> button.</summary>
    public static CommonButton TryAgain => new(1 << 18, MB_RESULT.IDTRYAGAIN);

    /// <summary>Gets a new <i>Yes</i> button.</summary>
    public static CommonButton Yes => new(TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_YES_BUTTON, MB_RESULT.IDYES);

    string INativeProvider<string>.GetNative() => _text;
}