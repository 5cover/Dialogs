using Scover.Dialogs.Parts;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

// State properties can be set at anytime. Disposable state properties are not disposed because they are not considered to be
// owned by the page.

// State is { get; set; }

public partial class Page
{
    private DialogIcon _footerIcon = DialogIcon.None;
    private DialogIcon _icon = DialogIcon.None;

    /// <summary>Gets or sets the content.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be show in the content area.</remarks>
    /// <value>The text shown in the content area. Default value is <see langword="null"/>.</value>
    public string? Content
    {
        get => _wrap.Content;
        set
        {
            _wrap.Content = value;
            SetElementText(TASKDIALOG_ELEMENTS.TDE_CONTENT, _wrap.pszContent);
        }
    }

    /// <summary>Gets or sets the footer icon.</summary>
    /// <value>The handle of the icon to show in the footer area of the page. Default value is <see cref="DialogIcon.None"/>.</value>
    public DialogIcon FooterIcon
    {
        get => _footerIcon;
        set
        {
            _footerIcon = value;
            value.SetFooterIcon(_wrap);
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
            SetElementText(TASKDIALOG_ELEMENTS.TDE_FOOTER, _wrap.pszFooter);
        }
    }

    /// <summary>Gets or sets the icon.</summary>
    /// <value>The handle of the icon to show in the content area of the page. Default value is <see cref="DialogIcon.None"/>.</value>
    public DialogIcon Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            value.SetMainIcon(_wrap);
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
            SetElementText(TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION, _wrap.pszMainInstruction);
        }
    }

    /// <summary>Gets the progress bar.</summary>
    /// <remarks>If the value is <see langword="null"/>, no progress bar will be shown.</remarks>
    /// <value>The progress bar of the page. Default value is <see langword="null"/>.</value>
    public ProgressBar? ProgressBar
    {
        get => _parts.GetPart<ProgressBar>();
        set => _parts.SetPart(_wrap, value);
    }
}