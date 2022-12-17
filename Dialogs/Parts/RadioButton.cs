using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog radio button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
public sealed class RadioButton : IUpdateRequester<IdControlUpdate>, INativeProvider<StrPtrUni>, INotificationHandler, IStateInitializer, IDisposable
{
    private readonly SafeLPWSTR _text;
    private bool _isEnabled = true;

    /// <summary>Initializes a new instance of the <see cref="RadioButton"/> class.</summary>
    /// <param name="text">The text of the radio button.</param>
    public RadioButton(string text) => (Text, _text) = (text, new(text));

    /// <summary>Event raised when the radio button is clicked.</summary>
    public event EventHandler? Clicked;
    event EventHandler<Action<IdControlUpdate>>? IUpdateRequester<IdControlUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<IdControlUpdate>>? UpdateRequested;

    /// <summary>Gets or sets whether this radio button is enabled.</summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            RequestIsEnabledUpdate();
        }
    }

    /// <summary>Gets the text of this radio button.</summary>
    public string Text { get; }

    /// <summary>Simulates a click on this radio button.</summary>
    public void Click() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_RADIO_BUTTON, update.ControlId));

    /// <inheritdoc/>
    public void Dispose() => _text.Dispose();

    StrPtrUni INativeProvider<StrPtrUni>.GetNative() => _text;

    HRESULT INotificationHandler.HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED)
        {
            Clicked.Raise(this);
        }
        return default;
    }

    void IStateInitializer.InitializeState() => RequestIsEnabledUpdate();

    private void OnUpdateRequested(Action<IdControlUpdate> update) => UpdateRequested?.Invoke(this, update);

    private void RequestIsEnabledUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_RADIO_BUTTON, update.ControlId, _isEnabled));
}