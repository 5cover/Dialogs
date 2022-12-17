using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog radio button controls. This class cannot be inherited.</summary>
/// <inheritdoc path="/remarks"/>
public sealed class RadioButtonCollection : IdControlCollection<RadioButton>, INotificationHandler
{
    private readonly DefaultRadioButton _defaultRadioButton;

    /// <summary>Initializes a new instance of the <see cref="RadioButtonCollection"/> class.</summary>
    /// <param name="defaultRadioButton">The default radio button. Default value is <see cref="DefaultRadioButton.First"/>.</param>
    public RadioButtonCollection(DefaultRadioButton? defaultRadioButton = null) : base((defaultRadioButton ?? DefaultRadioButton.First).RadioButton)
        => _defaultRadioButton = defaultRadioButton ?? DefaultRadioButton.First;

    /// <summary>Initializes a new instance of the <see cref="RadioButtonCollection"/> class.</summary>
    /// <param name="defaultItem">The default radio button.</param>
    public RadioButtonCollection(RadioButton defaultItem) : this(DefaultRadioButton.FromRadioButton(defaultItem))
    {
    }

    private protected override TASKDIALOG_FLAGS Flags => _defaultRadioButton.Flags;

    /// <summary>Adds a new radio button to the collection.</summary>
    /// <param name="text">The label.</param>
    public void Add(string text) => Add(new RadioButton(text));

    HRESULT INotificationHandler.HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED && GetControlFromId((int)wParam) is INotificationHandler notificationHandler)
        {
            return notificationHandler.HandleNotification(id, wParam, lParam);
        }
        return default;
    }

    private protected override void SetContainerProperties(in TASKDIALOGCONFIG container, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (container.pRadioButtons, container.cRadioButtons, container.nDefaultRadioButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}