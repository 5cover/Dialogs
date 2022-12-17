using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

internal sealed record NativeIcon(bool IsHICON, nint Handle);

/// <summary>An dialog icon control. Can be a standard or custom. This class cannot be inherited.</summary>
public sealed class DialogIcon : INativeProvider<NativeIcon>
{
    // todo : bar composition (requires preemptive testing) (see in KPreisser)

    private readonly NativeIcon _icon;

    /// <summary>Initializes a new instance of the <see cref="DialogIcon"/> class.</summary>
    /// <remarks>The caller is responsible for freeing the icon resource.</remarks>
    /// <param name="handle">The icon handle to use.</param>
    public DialogIcon(nint handle) => _icon = new(true, handle);

    private DialogIcon(TaskDialogIcon icon) => _icon = new(false, (nint)icon);

    private DialogIcon() => _icon = new(false, default);

    /// <summary>Gets an icon consisting of a white X in a circle with a red background.</summary>
    public static DialogIcon Error { get; } = new(TaskDialogIcon.TD_ERROR_ICON);

    /// <summary>Gets an icon consisting of a consisting of a lowercase letter i in a circle.</summary>
    public static DialogIcon Information { get; } = new(TaskDialogIcon.TD_INFORMATION_ICON);

    /// <summary>Gets a special <see cref="DialogIcon"/> instance that represents the absence of an icon.</summary>
    public static DialogIcon None { get; } = new();

    /// <summary>Gets an icon consisting of a consisting of an user account control (UAC) shield.</summary>
    public static DialogIcon Shield { get; } = new(TaskDialogIcon.TD_SHIELD_ICON);

    /// <summary>
    /// Gets an icon consisting of a consisting of an user account control (UAC) shield and shows a blue bar around the icon.
    /// </summary>
    public static DialogIcon ShieldBlueBar { get; } = new(TaskDialogIcon.TD_SHIELDBLUE_ICON);

    /// <summary>Gets an icon consisting of a consisting of a white X in a red shield and shows a red bar around the icon.</summary>
    public static DialogIcon ShieldErrorRedBar { get; } = new(TaskDialogIcon.TD_SECURITYERROR_ICON);

    /// <summary>
    /// Gets an icon consisting of a consisting of an user account control (UAC) shield and shows a gray bar around the icon.
    /// </summary>
    public static DialogIcon ShieldGrayBar { get; } = new(TaskDialogIcon.TD_SHIELDGRAY_ICON);

    /// <summary>
    /// Gets an icon consisting of a consisting of a white tick in a green shield and shows a green bar around the icon.
    /// </summary>
    public static DialogIcon ShieldSuccessGreenBar { get; } = new(TaskDialogIcon.TD_SECURITYSUCCESS_ICON);

    /// <summary>
    /// Gets an icon consisting of a consisting of an exclamation point in a yellow shield and shows a yellow bar around the icon.
    /// </summary>
    public static DialogIcon ShieldWarningYellowBar { get; } = new(TaskDialogIcon.TD_SECURITYWARNING_ICON);

    /// <summary>Gets an icon consisting of a consisting of an exclamation point in a triangle with a yellow background.</summary>
    public static DialogIcon Warning { get; } = new(TaskDialogIcon.TD_WARNING_ICON);

    NativeIcon INativeProvider<NativeIcon>.GetNative() => _icon;
}