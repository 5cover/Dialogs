using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog control that induces commitment.</summary>
public abstract class CommitControl : IUpdateRequester<IdControlUpdate>, IStateInitializer
{
    private bool _isEnabled = true;

    private bool _showShieldIcon;

    event EventHandler<Action<IdControlUpdate>>? IUpdateRequester<IdControlUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<IdControlUpdate>>? UpdateRequested;

    /// <summary>Gets or sets whether this commit control is enabled.</summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                RequestShowShieldIconUpdate();
            }
        }
    }

    /// <summary>Gets or sets whether an User Account Control (UAC) shield icon is displayed near the commit control.</summary>
    public bool RequiresElevation
    {
        get => _showShieldIcon;
        set
        {
            if (_showShieldIcon != value)
            {
                _showShieldIcon = value;
                RequestIsEnabledUpdate();
            }
        }
    }

    /// <summary>Simulates a click on this button.</summary>
    public void Click() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_BUTTON, update.ControlId));

    void IStateInitializer.InitializeState()
    {
        RequestIsEnabledUpdate();
        RequestShowShieldIconUpdate();
    }

    private protected void OnUpdateRequested(Action<IdControlUpdate> update) => UpdateRequested?.Invoke(this, update);

    private void RequestIsEnabledUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_BUTTON, update.ControlId, _isEnabled));

    private void RequestShowShieldIconUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE, update.ControlId, _showShieldIcon));
}