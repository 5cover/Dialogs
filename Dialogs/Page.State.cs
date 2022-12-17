using Scover.Dialogs.Parts;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

// State properties can be modified at anytime. When set to a different value, they raise the UpdateRequest event.

public partial class Page
{
    /// <summary>Gets or sets the content.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be show in the content area.</remarks>
    /// <value>The text shown in the content area. Default value is <see langword="null"/>.</value>
    public string? Content
    {
        get => _wrap.Content;
        set
        {
            _wrap.Content = value;
            RequestTextUpdate(TASKDIALOG_ELEMENTS.TDE_CONTENT, _wrap.pszContent);
        }
    }

    /// <summary>Gets or sets the footer icon.</summary>
    /// <value>The handle of the icon to show in the footer area of the page. Default value is <see langword="null"/>.</value>
    public DialogIcon FooterIcon
    {
        get => _footerIcon;
        set
        {
            _footerIcon = value;
            var icon = value.GetNative();
            _wrap.footerIcon = icon.Handle;
            _wrap.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_FOOTER, icon.IsHICON);
            _ = OwnerDialog.SendMessage(TaskDialogMessage.TDM_UPDATE_ICON, TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_FOOTER, _wrap.footerIcon);
        }
    }

    /// <summary>Gets or sets the footer text.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be shown in the footer area.</remarks>
    /// <value>The text to show in the footer area. Default value is <see langword="null"/>.</value>
    public string? FooterText
    {
        get => _wrap.Footer;
        set
        {
            _wrap.Footer = value;
            RequestTextUpdate(TASKDIALOG_ELEMENTS.TDE_FOOTER, _wrap.pszFooter);
        }
    }

    /// <summary>Gets or sets the icon.</summary>
    /// <value>The handle of the icon to show in the content area of the page. Default value is <see langword="null"/>.</value>
    public DialogIcon Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            var icon = value.GetNative();
            _wrap.mainIcon = icon.Handle;
            _wrap.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN, icon.IsHICON);
            _ = OwnerDialog.SendMessage(TaskDialogMessage.TDM_UPDATE_ICON, TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_MAIN, _wrap.footerIcon);
        }
    }

    /// <summary>Gets or sets the main instruction.</summary>
    /// <remarks>If the value is <see langword="null"/>, no main instruction will be shown.</remarks>
    /// <value>The main instruction of the page. Default value is <see langword="null"/>.</value>
    public string? MainInstruction
    {
        get => _wrap.MainInstruction;
        set
        {
            _wrap.MainInstruction = value;
            RequestTextUpdate(TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION, _wrap.pszMainInstruction);
        }
    }
}