using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog radio button controls.</summary>
/// <remarks>
/// This class cannot be inherited and implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.
/// </remarks>
public sealed class RadioButtonCollection : IdControlCollection<RadioButton>
{
    private readonly DefaultRadioButton _defaultRadioButton;

    /// <summary>Initializes a new empty <see cref="RadioButtonCollection"/>.</summary>
    /// <param name="defaultRadioButton">The default radio button. Default value is <see cref="DefaultRadioButton.First"/>.</param>
    public RadioButtonCollection(DefaultRadioButton? defaultRadioButton = null) : base((defaultRadioButton ?? DefaultRadioButton.First).RadioButton)
        => (_defaultRadioButton, Selected) = (defaultRadioButton ?? DefaultRadioButton.First, DefaultItem);

    /// <summary>Initializes a new empty <see cref="RadioButtonCollection"/>.</summary>
    /// <param name="defaultItem">The default radio button.</param>
    public RadioButtonCollection(RadioButton defaultItem) : this(DefaultRadioButton.FromRadioButton(defaultItem))
    {
    }

    /// <summary>Gets a reference to the currently selected radio button.</summary>
    /// <value>The currently selected radio button, or <see langword="null"/> if no radio button is currently selected.</value>
    public RadioButton? Selected { get; private set; }

    private protected override TASKDIALOG_FLAGS Flags => _defaultRadioButton.Flags;

    /// <summary>Adds a new radio button to the collection.</summary>
    /// <param name="text">The label.</param>
    public void Add(string text) => Add(new RadioButton(text));

    /// <remarks>
    /// <inheritdoc path="/remarks"/>
    /// <item>
    /// <term><see cref="TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED"/></term>
    /// <term>Forwards the notification to the clicked radio button</term>
    /// <term><see cref="RadioButton.HandleNotification(Notification)"/></term>
    /// </item>
    /// </remarks>
    /// <returns>
    /// The notification is forwarded to all items. <see langword="null"/> if none of the items had a meaningful value to
    /// return, the notification-specific return value otherwise.
    /// </returns>
    /// <inheritdoc/>
    internal override HRESULT? HandleNotification(Notification notif)
    {
        _ = base.HandleNotification(notif);
        if (notif.Id is TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED)
        {
            var control = GetControlFromId((int)notif.WParam);
            if (control is not null)
            {
                Selected = control;
            }
            return control?.HandleNotification(notif);
        }
        return this.ForwardNotification(notif);
    }

    private protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pRadioButtons, config.cRadioButtons, config.nDefaultRadioButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}