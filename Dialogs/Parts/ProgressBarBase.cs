using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/*var progressBar = User32.FindWindowEx(Handle, HWND.NULL, "PROGRESS_CLASS", null);
User32.GetWindowRect(progressBar, out RECT progressBarRect);
return progressBarRect.Width;*/

/// <summary>Base class for progress bar dialog controls.</summary>
public abstract class ProgressBarBase : ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, IStateInitializer
{
    private const ProgressBarState DefaultState = ProgressBarState.Normal;
    private ProgressBarState _state = DefaultState;

    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }

    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets or sets the progress bar state.</summary>
    /// <remarks>Default value is <see cref="ProgressBarState.Normal"/>.</remarks>
    public ProgressBarState State
    {
        get => _state;
        set
        {
            _state = value;
            RequestStateUpdate();
        }
    }

    void IStateInitializer.InitializeState() => InitializeState();

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container) => SetIn(container);

    private protected virtual void InitializeState() => RequestStateUpdate();

    private protected void OnUpdateRequested(Action<PageUpdate> update) => UpdateRequested?.Invoke(this, update);

    private protected abstract void SetIn(in TASKDIALOGCONFIG container);

    private void RequestStateUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_STATE, _state));
}