using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog progress bar control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ProgressBar : ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, IStateInitializer
{
    private int _marqueeSpeed = 30;
    private ushort _maximum = 100;
    private ushort _minimum;
    private ProgressBarMode _mode = ProgressBarMode.Normal;
    private ProgressBarState _state = ProgressBarState.Normal;
    private int _value;
    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets or sets the progress bar marquee speed.</summary>
    /// <remarks>
    /// Since the marquee animation speed is a time period, setting the value to a higher number results in a slower speed and a
    /// lower number results in a faster speed.
    /// </remarks>
    /// <value>The time, in milliseconds, between marque progress bar animation updates. Default value is 30.</value>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int MarqueeSpeed
    {
        get => _marqueeSpeed;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Must be greater than zero");
            }
            _marqueeSpeed = value;
            RequestSpeedUpdate();
        }
    }

    /// <summary>Gets or sets the maximum progress bar value.</summary>
    /// <remarks>Default value is 100. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="OverflowException">The value is less than 0 or greater than 65535.</exception>
    public int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = checked((ushort)value);
            RequestRangeUpdate();
        }
    }

    /// <summary>Gets or sets the minimum progress bar value.</summary>
    /// <remarks>Default value is 0. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="OverflowException">The value is less than 0 or greater than 65535.</exception>
    public int Minimum
    {
        get => _minimum;
        set
        {
            _minimum = checked((ushort)value);
            RequestRangeUpdate();
        }
    }

    /// <summary>Gets or sets the mode.</summary>
    /// <remarks>Default value is <see cref="ProgressBarMode.Normal"/>.</remarks>
    public ProgressBarMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            if (value is ProgressBarMode.Marquee)
            {
                // Set underlying state to normal -- This is because marquee doesn't support abnormal states.
                RequestStateUpdate(ProgressBarState.Normal);
            }
            RequestModeUpdate();
            if (value is ProgressBarMode.Normal)
            {
                RequestValueUpdate(); // Needed for transitioning from Marquee to Normal
                RequestStateUpdate(); // Update the underlying state after leaving Marquee mode
            }
        }
    }

    /// <summary>Gets or sets the progress bar state.</summary>
    /// <remarks>Default value is <see cref="ProgressBarState.Normal"/>.</remarks>
    public ProgressBarState State
    {
        get => _state;
        set
        {
            _state = value;
            if (Mode is ProgressBarMode.Normal) // Marquee doesn't support abnormal states.
            {
                RequestStateUpdate();
            }
        }
    }

    /// <summary>Gets or sets the current progress bar value.</summary>
    /// <remarks>Default value is 0.</remarks>
    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, Minimum, Maximum);
            RequestValueUpdate();
            if (State is not ProgressBarState.Normal)
            {
                RequestValueUpdate(); // Needed to keep the value synced when state is abnormal for some reason.
            }
        }
    }

    void IStateInitializer.InitializeState()
    {
        RequestModeUpdate();
        RequestRangeUpdate();
        RequestSpeedUpdate();
        RequestStateUpdate();
        RequestValueUpdate();
    }

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container)
    {
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR, _mode is ProgressBarMode.Normal);
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee);
        OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_MARQUEE_PROGRESS_BAR, true));
    }

    private void OnUpdateRequested(Action<PageUpdate> update) => UpdateRequested?.Invoke(this, update);

    private void RequestModeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee));

    private void RequestRangeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_RANGE, 0, Macros.MAKELONG(_minimum, _maximum)));

    private void RequestSpeedUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_MARQUEE, true, (uint)_marqueeSpeed));

    private void RequestStateUpdate() => RequestStateUpdate(_state);

    private void RequestStateUpdate(ProgressBarState state) => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_STATE, state));

    private void RequestValueUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_POS, _value));
}