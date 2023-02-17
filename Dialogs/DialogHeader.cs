using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A header bar shown in top area of a dialog.</summary>
/// <remarks>Only available on Windows 8 and later. This class cannot be inherited.</remarks>
public sealed class DialogHeader : DialogControl<PageUpdateInfo>
{
    private readonly TaskDialogIcon _id;

    private DialogHeader(TaskDialogIcon id) => _id = id;

    /// <summary>Gets the blue header.</summary>
    public static DialogHeader Blue { get; } = new(TaskDialogIcon.TD_SHIELDBLUE_ICON);

    /// <summary>Gets the gray header.</summary>
    public static DialogHeader Gray { get; } = new(TaskDialogIcon.TD_SHIELDGRAY_ICON);

    /// <summary>Gets the green header.</summary>
    public static DialogHeader Green { get; } = new(TaskDialogIcon.TD_SECURITYSUCCESS_ICON);

    /// <summary>
    /// Gets the <see cref="DialogHeader"/> instance that represents the absence of a header.
    /// </summary>
    public static DialogHeader None { get; } = new(default);

    /// <summary>Gets the red header.</summary>
    public static DialogHeader Red { get; } = new(TaskDialogIcon.TD_SECURITYERROR_ICON);

    /// <summary>Gets the yellow header.</summary>
    public static DialogHeader Yellow { get; } = new(TaskDialogIcon.TD_SECURITYWARNING_ICON);

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        if (!config.dwFlags.HasFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN))
        {
            config.mainIcon = (nint)_id;
        }
    }
}