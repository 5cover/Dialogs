using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A style of buttons.</summary>
public enum ButtonStyle
{
    /// <summary>Buttons are shown as push buttons.</summary>
    PushButtons,

    /// <summary>Buttons are shown as command links.</summary>
    CommandLinks,

    /// <summary>Buttons are shown as command links without the arrow icon.</summary>
    CommandLinksNoIcon
}

/// <summary>A collection of buttons.</summary>
/// <inheritdoc path="//remarks"/>
public sealed class ButtonCollection : IdControlCollection<ButtonBase>
{
    /// <param name="defaultItem">
    /// The default button. If <see langword="null"/>, the default button will be the first item of the
    /// collection.
    /// </param>
    /// <param name="style">
    /// The button style to use. Default value is <see cref="ButtonStyle.PushButtons"/>.
    /// </param>
    /// <param name="items">The items already in the collection.</param>
    public ButtonCollection(ButtonStyle style = ButtonStyle.PushButtons, ButtonBase? defaultItem = null, IList<ButtonBase>? items = null) : base(items, defaultItem) => Style = style;

    /// <summary>Gets or sets the button style to use for this collection.</summary>
    public ButtonStyle Style { get; set; }

    ///<inheritdoc/>
    protected override TASKDIALOG_FLAGS Flags => Style switch
    {
        ButtonStyle.PushButtons => default,
        ButtonStyle.CommandLinks => TDF_USE_COMMAND_LINKS,
        ButtonStyle.CommandLinksNoIcon => TDF_USE_COMMAND_LINKS_NO_ICON,
        _ => throw Style.InvalidEnumArgumentException()
    };

    /// <inheritdoc/>
    protected override int StartId => CommonButton.MaxId + 1;

    /// <summary>Adds a new push button or command link to the collection.</summary>
    /// <param name="text">The push button or command link text.</param>
    /// <param name="note">The command link supwplemental instruction.</param>
    public void Add(string text, string? note = null) => Add(new Button(text, note));

    /// <remarks>
    /// <list type="table">
    /// <inheritdoc path="//remarks//listheader"/><inheritdoc path="//remarks//item"/>
    /// <item>
    /// <term><see cref="TDN_BUTTON_CLICKED"/></term>
    /// <term>Forwards the notification to the clicked button.</term>
    /// <term><see cref="ButtonBase.HandleNotification(Notification)"/></term>
    /// </item>
    /// <item>
    /// <term>Anything else</term>
    /// <term>Forwards the notification to all items.</term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <inheritdoc/>
    internal override HRESULT HandleNotification(Notification notif)
    {
        _ = base.HandleNotification(notif);
        if (notif.Id is TDN_BUTTON_CLICKED)
        {
            return GetItem((int)notif.WParam) is { } button ? button.HandleNotification(notif) : default;
        }
        _ = this.ForwardNotification(notif);
        return default;
    }

    /// <inheritdoc/>
    protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pButtons, config.cButtons, config.nDefaultButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}