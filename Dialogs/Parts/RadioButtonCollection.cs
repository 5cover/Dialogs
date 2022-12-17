using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog radio button controls.</summary>
public sealed class RadioButtonCollection : IdControlCollection<RadioButton>, INotificationHandler
{
    /// <summary>Initializes a new instance of the <see cref="RadioButtonCollection"/> class.</summary>
    /// <param name="defaultRadioButton">
    /// The default radio button. If <see langword="null"/>, there will be no default radio button.
    /// </param>
    public RadioButtonCollection(RadioButton? defaultRadioButton = null) : base(defaultRadioButton)
    {
    }

    private protected override TASKDIALOG_FLAGS Flags => DefaultItem is null ? TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON : default;

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