using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A collection of dialog radio button controls.</summary>
/// <inheritdoc path="//remarks"/>
public sealed class RadioButtonCollection : IdControlCollection<RadioButton>
{
    private readonly DefaultRadioButton? _defaultItemStrategy;

    /// <param name="defaultItemStrategy">The default radio button strategy.</param>
    /// <param name="items">The initial items to add to the collection.</param>
    public RadioButtonCollection(DefaultRadioButton defaultItemStrategy, IList<RadioButton>? items = null) : base(items, null)
        => _defaultItemStrategy = defaultItemStrategy;

    /// <param name="defaultItem">The default radio button.</param>
    /// <param name="items">The initial items to add to the collection.</param>
    public RadioButtonCollection(RadioButton defaultItem, IList<RadioButton>? items = null) : base(items, defaultItem)
        => Selected = defaultItem;

    /// <param name="items">The initial items to add to the collection.</param>
    public RadioButtonCollection(IList<RadioButton>? items = null) : this(DefaultRadioButton.First, items)
    {
    }

    /// <summary>Gets a reference to the currently selected radio button.</summary>
    /// <value>
    /// The currently selected radio button, or <see langword="null"/> if no radio button is currently
    /// selected.
    /// </value>
    public RadioButton? Selected { get; }

    /// <inheritdoc/>
    protected override TASKDIALOG_FLAGS Flags => _defaultItemStrategy?.Flags ?? 0;

    /// <summary>Adds a new radio button to the collection.</summary>
    /// <param name="text">The label.</param>
    public void Add(string text) => Add(new RadioButton(text));

    /// <remarks>
    /// <list type="table">
    /// <inheritdoc path="//remarks//listheader"/><inheritdoc path="//remarks//item"/>
    /// <item>
    /// <term><see cref="TDN_RADIO_BUTTON_CLICKED"/></term>
    /// <term>Forwards the notification to the clicked button.</term>
    /// <term><see cref="RadioButton.HandleNotification(Notification)"/></term>
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
        if (notif.Id is TDN_RADIO_BUTTON_CLICKED && GetControlFromId((int)notif.WParam) is { } radioButton)
        {
            return radioButton.HandleNotification(notif);
        }
        _ = this.ForwardNotification(notif);
        return default;
    }

    /// <inheritdoc/>
    protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pRadioButtons, config.cRadioButtons, config.nDefaultRadioButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}