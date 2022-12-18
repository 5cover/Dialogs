using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>An dialog icon control. Can be standard or custom.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class DialogIcon
{
    // todo : bar composition (requires preemptive testing) (see in KPreisser)

    private readonly nint _handle;
    private readonly bool _isHIcon;

    private DialogIcon(nint handle, bool isHIcon) => (_handle, _isHIcon) = (handle, isHIcon);

    private DialogIcon(TaskDialogIcon icon) => (_handle, _isHIcon) = ((nint)icon, false);

    private DialogIcon() => (_handle, _isHIcon) = (default, false);

    /// <summary>Gets the icon consisting of a white X in a circle with a red background.</summary>
    public static DialogIcon Error { get; } = new(TaskDialogIcon.TD_ERROR_ICON);

    /// <summary>Gets the icon consisting of a lowercase letter i in a circle.</summary>
    public static DialogIcon Information { get; } = new(TaskDialogIcon.TD_INFORMATION_ICON);

    /// <summary>Gets a special <see cref="DialogIcon"/> instance that represents the absence of an icon.</summary>
    public static DialogIcon None { get; } = new();

    /// <summary>Gets the icon consisting of an user account control (UAC) shield.</summary>
    public static DialogIcon Shield { get; } = new(TaskDialogIcon.TD_SHIELD_ICON);

    /// <summary>Gets the icon consisting of an user account control (UAC) shield and shows a blue bar around the icon.</summary>
    public static DialogIcon ShieldBlueBar { get; } = new(TaskDialogIcon.TD_SHIELDBLUE_ICON);

    /// <summary>Gets the icon consisting of a white X in a red shield and shows a red bar around the icon.</summary>
    public static DialogIcon ShieldErrorRedBar { get; } = new(TaskDialogIcon.TD_SECURITYERROR_ICON);

    /// <summary>Gets the icon consisting of an user account control (UAC) shield and shows a gray bar around the icon.</summary>
    public static DialogIcon ShieldGrayBar { get; } = new(TaskDialogIcon.TD_SHIELDGRAY_ICON);

    /// <summary>Gets the icon consisting of a white tick in a green shield and shows a green bar around the icon.</summary>
    public static DialogIcon ShieldSuccessGreenBar { get; } = new(TaskDialogIcon.TD_SECURITYSUCCESS_ICON);

    /// <summary>Gets the icon consisting of an exclamation point in a yellow shield and shows a yellow bar around the icon.</summary>
    public static DialogIcon ShieldWarningYellowBar { get; } = new(TaskDialogIcon.TD_SECURITYWARNING_ICON);

    /// <summary>Gets the icon consisting of an exclamation point in a triangle with a yellow background.</summary>
    public static DialogIcon Warning { get; } = new(TaskDialogIcon.TD_WARNING_ICON);

    /// <summary>Creates a new <see cref="DialogIcon"/> from an icon handle.</summary>
    /// <param name="hIcon">The icon handle to use. The caller is responsible for freeing the icon resource.</param>
    /// <returns>A new instance of the <see cref="DialogIcon"/> class.</returns>
    public static DialogIcon FromHandle(nint hIcon) => new(hIcon, true);

    /// <summary>Creates a new <see cref="DialogIcon"/> from an icon ID in <c>imageres.dll</c>.</summary>
    /// <param name="iconId">The ID of the icon in <c>imageres.dll</c>.</param>
    /// <returns>A new instance of the <see cref="DialogIcon"/> class.</returns>
    public static DialogIcon FromId(int iconId) => new(Macros.MAKEINTRESOURCE(iconId).id, false);

    internal void SetFooterIcon(in TASKDIALOGCONFIG config)
    {
        config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_FOOTER, _isHIcon);
        config.footerIcon = _handle;
    }

    internal void SetMainIcon(in TASKDIALOGCONFIG config)
    {
        config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN, _isHIcon);
        config.mainIcon = _handle;
    }
}