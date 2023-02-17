using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

// States properties can be set at anytime. Disposable state properties are not disposed because they are
// not considered to be owned by the page.

// State is { get; set; }

// Non-nullable reference types are used for element texts because SET_ELEMENT_TEXT and UPDATE_ELEMENT_TEXT
// do not work on null string pointers.

public partial class Page
{
    private DialogIcon _footerIcon = DialogIcon.None;
    private DialogIcon _icon = DialogIcon.None;

    /// <summary>Gets or sets the content.</summary>
    /// <remarks>
    /// If the value is <see cref="string.Empty"/>, no text will be show in the content area.
    /// </remarks>
    /// <value>The text shown in the content area. Default value is <see cref="string.Empty"/>.</value>
    public string Content
    {
        get => _config.Content ?? "";
        set
        {
            _config.Content = value;
            SetElementText(TASKDIALOG_ELEMENTS.TDE_CONTENT, _config.pszContent);
        }
    }

    /// <summary>Gets or sets the footer icon.</summary>
    /// <value>
    /// The handle of the icon to show in the footer area of the page. Default value is <see
    /// cref="DialogIcon.None"/>.
    /// </value>
    public DialogIcon FooterIcon
    {
        get => _footerIcon;
        set
        {
            DenyHIconIDTransition(_footerIcon, value);
            _footerIcon = value;
            _config.footerIcon = value.Handle;
            _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_FOOTER, value.IsHIcon);
            OnUpdateRequested(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_UPDATE_ICON, TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_FOOTER, value.Handle));
        }
    }

    /// <summary>Gets or sets the footer text.</summary>
    /// <remarks>
    /// If the value is <see cref="string.Empty"/>, no text will be shown in the footer area.
    /// </remarks>
    /// <value>The text to show in the footer area. Default value is <see cref="string.Empty"/>.</value>
    public string FooterText
    {
        get => _config.Footer ?? "";
        set
        {
            _config.Footer = value;
            SetElementText(TASKDIALOG_ELEMENTS.TDE_FOOTER, _config.pszFooter);
        }
    }

    /// <summary>Gets or sets the icon.</summary>
    /// <value>
    /// The handle of the icon to show in the content area of the page. Default value is <see
    /// cref="DialogIcon.None"/>.
    /// </value>
    public DialogIcon Icon
    {
        get => _icon;
        set
        {
            DenyHIconIDTransition(_icon, value);
            _icon = value;
            _config.mainIcon = Icon.Handle;
            _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN, Icon.IsHIcon);
            OnUpdateRequested(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_UPDATE_ICON, TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_MAIN, Icon.Handle));
        }
    }

    /// <summary>Gets or sets the main instruction.</summary>
    /// <remarks>If the value is <see cref="string.Empty"/>, no main instruction will be shown.</remarks>
    /// <value>The main instruction of the page. Default value is <see cref="string.Empty"/>.</value>
    public string MainInstruction
    {
        get => _config.MainInstruction ?? "";
        set
        {
            _config.MainInstruction = value;
            SetElementText(TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION, _config.pszMainInstruction);
        }
    }

    private void DenyHIconIDTransition(DialogIcon current, DialogIcon value)
    {
        if (_handleSent && current.IsHIcon != value.IsHIcon)
        {
            throw new ArgumentException("Cannot transition between HIcon and ID while the dialog is shown.");
        }
    }
}