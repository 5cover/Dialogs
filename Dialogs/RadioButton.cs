using System.Diagnostics;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A dialog radio button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(_nativeText)}}}")]
public sealed class RadioButton : DialogControl<IdControlUpdateInfo>, ITextControl, IDisposable, IEquatable<RadioButton?>
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
            RequestUpdate(UpdateIsEnabled);
        }
    }

    /// <summary>Gets the text of this radio button.</summary>
    public string Text { get; }

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <summary>Simulates a click on this radio button.</summary>
    public void Click() => RequestUpdate(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_RADIO_BUTTON, info.ControlId));

    /// <inheritdoc/>
    public void Dispose() => _nativeText.Dispose();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as RadioButton);

    /// <inheritdoc/>
    public bool Equals(RadioButton? other) => other is not null && Text == other.Text;

    /// <inheritdoc/>
    public override int GetHashCode() => Text.GetHashCode();

    /// <remarks>
    /// <inheritdoc path="/remarks"/>
    /// <item>
    /// <term><see cref="TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED"/></term>
    /// <term>Raises <see cref="Clicked"/></term>
    /// <term><see langword="null"/></term>
    /// </item>
    /// </remarks>
    /// <inheritdoc/>
    internal override HRESULT? HandleNotification(Notification notif)
    {
        if (notif.Id is TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
        return base.HandleNotification(notif);
    }

    private protected override void InitializeState() => RequestUpdate(UpdateIsEnabled);

    private void UpdateIsEnabled(IdControlUpdateInfo info) => info.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_RADIO_BUTTON, info.ControlId, _isEnabled);
}