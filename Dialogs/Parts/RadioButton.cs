using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog radio button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
public sealed class RadioButton : DialogControl<IdControlUpdateInfo>, ITextControl, IDisposable
{
    private readonly SafeLPWSTR _nativeText;
    private bool _isEnabled = true;

    /// <param name="text">The text of the radio button.</param>
    public RadioButton(string text) => (Text, _nativeText) = (text, new(text));

    /// <summary>Event raised when the radio button is clicked.</summary>
    public event EventHandler? Clicked;

    /// <summary>Gets or sets whether this radio button is enabled.</summary>
    /// <remarks>Default value is <see langword="true"/>.</remarks>
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

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <summary>Simulates a click on this radio button.</summary>
    public void Click() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_RADIO_BUTTON, update.ControlId));

    /// <inheritdoc/>
    public void Dispose() => _nativeText.Dispose();

    internal override HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED)
        {
            Clicked.Raise(this);
        }
        return base.HandleNotification(id, wParam, lParam);
    }

    private protected override void InitializeState() => RequestIsEnabledUpdate();

    private void RequestIsEnabledUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_RADIO_BUTTON, update.ControlId, _isEnabled));
}