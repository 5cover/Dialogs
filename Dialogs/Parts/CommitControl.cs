using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog control that induces commitment.</summary>
public abstract class CommitControl : IUpdateRequester<IdControlUpdate>, IStateInitializer
{
    private bool _isEnabled = true;

    private bool _requiresElevation;

    event EventHandler<Action<IdControlUpdate>>? IUpdateRequester<IdControlUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<IdControlUpdate>>? UpdateRequested;

    /// <summary>Gets or sets whether this commit control is enabled.</summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            RequestIsEnabledUpdate();
        }
    }

    /// <summary>Gets or sets whether an User Account Control (UAC) shield icon is displayed near the commit control.</summary>
    public bool RequiresElevation
    {
        get => _requiresElevation;
        set
        {
            _requiresElevation = value;
            RequestElevationUpdate();
        }
    }

    /// <summary>Simulates a click on this button.</summary>
    public void Click() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_BUTTON, update.ControlId));

    void IStateInitializer.InitializeState()
    {
        RequestElevationUpdate();
        RequestIsEnabledUpdate();
    }

    private protected void OnUpdateRequested(Action<IdControlUpdate> update) => UpdateRequested?.Invoke(this, update);

    private void RequestElevationUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE, update.ControlId, _requiresElevation));

    private void RequestIsEnabledUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_BUTTON, update.ControlId, _isEnabled));
}