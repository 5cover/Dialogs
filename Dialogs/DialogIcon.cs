using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>An dialog icon control. Can be standard or custom.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class DialogIcon
{
    private readonly bool _isHIcon;

    private readonly nint _handle;

    private DialogIcon(nint handle, bool isHIcon) => (_handle, _isHIcon) = (handle, isHIcon);

    private DialogIcon(TaskDialogIcon icon) => (_handle, _isHIcon) = ((nint)icon, false);

    /// <summary>Gets the icon consisting of a white X in a circle with a red background.</summary>
    public static DialogIcon Error { get; } = new(TD_ERROR_ICON);

    /// <summary>Gets the icon consisting of a white X in a red shield.</summary>
    public static DialogIcon ErrorShield { get; } = new(TD_SECURITYERROR_ICON);

    /// <summary>Gets the icon consisting of a lowercase letter i in a circle.</summary>
    public static DialogIcon Information { get; } = new(TD_INFORMATION_ICON);

    /// <summary>
    /// Gets a special <see cref="DialogIcon"/> instance that represents the absence of an icon.
    /// </summary>
    public static DialogIcon None { get; } = new(default, false);

    /// <summary>Gets the icon consisting of an user account control (UAC) shield.</summary>
    public static DialogIcon Shield { get; } = new(TD_SHIELD_ICON);

    /// <summary>Gets the icon consisting of a white tick in a green shield.</summary>
    public static DialogIcon SuccessShield { get; } = new(TD_SECURITYSUCCESS_ICON);

    /// <summary>Gets the icon consisting of an exclamation point in a triangle.</summary>
    public static DialogIcon Warning { get; } = new(TD_WARNING_ICON);

    /// <summary>Gets the icon consisting of an exclamation point in a yellow shield.</summary>
    public static DialogIcon WarningShield { get; } = new(TD_SECURITYWARNING_ICON);

    /// <summary>Creates a new <see cref="DialogIcon"/> from an icon handle.</summary>
    /// <param name="hIcon">
    /// The icon handle to use. The caller is responsible for freeing the icon resource.
    /// </param>
    /// <returns>A new instance of the <see cref="DialogIcon"/> class.</returns>
    public static DialogIcon FromHandle(nint hIcon) => new(hIcon, true);

    /// <summary>Creates a new <see cref="DialogIcon"/> from an icon ID in <c>imageres.dll</c>.</summary>
    /// <param name="iconId">The ID of the icon in <c>imageres.dll</c>.</param>
    /// <returns>A new instance of the <see cref="DialogIcon"/> class.</returns>
    public static DialogIcon FromId(int iconId) => new(Macros.MAKEINTRESOURCE(iconId).id, false);

    internal void SetIn(in TASKDIALOGCONFIG config, TASKDIALOG_ICON_ELEMENTS element)
    {
        if (element is TDIE_ICON_MAIN)
        {
            config.mainIcon = _handle;
            config.dwFlags.SetFlag(TDF_USE_HICON_MAIN, _isHIcon);
        }
        else
        {
            config.footerIcon = _handle;
            config.dwFlags.SetFlag(TDF_USE_HICON_FOOTER, _isHIcon);
        }
    }

    internal bool IsHotChangeLegal(DialogIcon possibleNewValue) => _isHIcon == possibleNewValue._isHIcon;

    /// <inheritdoc/>
    internal Action<PageUpdateInfo> GetUpdate(TASKDIALOG_ICON_ELEMENTS element) => info => info.Dialog.SendMessage(TDM_UPDATE_ICON, element, _handle);
}