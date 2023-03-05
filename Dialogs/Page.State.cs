namespace Scover.Dialogs;

// States properties can be set at anytime. Disposable state properties are not disposed because they are
// not considered to be owned by the page.

// State is { get; set; }

// Non-nullable reference types are used for element texts because SET_ELEMENT_TEXT and UPDATE_ELEMENT_TEXT
// do not work on null string pointers.

public partial class Page
{
    private DialogIcon _icon = DialogIcon.None, _footerIcon = DialogIcon.None;

    /// <summary>Gets or sets the content.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be show in the content area.</remarks>
    /// <value>The text shown in the content area. Default value is <see langword="null"/>.</value>
    public string? Content
    {
        get => _content;
        set
        {
            value.SetAsValueOf(ref _content);
            SetElementText(TDE_CONTENT, _content);
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
            DenyIllegalHotChange(_footerIcon, value);
            OnUpdateRequested(value.GetUpdate(TDIE_ICON_FOOTER));
            _footerIcon = value;
        }
    }

    /// <summary>Gets or sets the footer text.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be shown in the footer area.</remarks>
    /// <value>The text to show in the footer area. Default value is <see langword="null"/>.</value>
    public string? FooterText
    {
        get => _footerText;
        set
        {
            value.SetAsValueOf(ref _footerText);
            SetElementText(TDE_FOOTER, _footerText);
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
            DenyIllegalHotChange(_icon, value);
            OnUpdateRequested(value.GetUpdate(TDIE_ICON_MAIN));
            _icon = value;
        }
    }

    /// <summary>Gets or sets the main instruction.</summary>
    /// <remarks>If the value is <see langword="null"/>, no main instruction will be shown.</remarks>
    /// <value>The main instruction of the page. Default value is <see langword="null"/>.</value>
    public string? MainInstruction
    {
        get => _mainInstruction;
        set
        {
            value.SetAsValueOf(ref _mainInstruction);
            SetElementText(TDE_MAIN_INSTRUCTION, _mainInstruction);
        }
    }

    private void DenyIllegalHotChange(DialogIcon current, DialogIcon value)
    {
        if (IsShown && !current.IsHotChangeLegal(value))
        {
            throw new InvalidOperationException("Cannot transition between HIcon and ID while the dialog is shown.");
        }
    }
}