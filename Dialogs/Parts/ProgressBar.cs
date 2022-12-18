﻿using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog progress bar control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ProgressBar : DialogControl<PageUpdateInfo>
{
    private int _marqueeInterval = 30;
    private ushort _maximum = 100;
    private ushort _minimum;
    private ProgressBarMode _mode = ProgressBarMode.Normal;
    private ProgressBarState _state = ProgressBarState.Normal;
    private int _value;

    /// <summary>Gets or sets the progress bar marquee interval.</summary>
    /// <remarks>
    /// Since the marquee animation interval is a time period, setting the value to a higher number results in a slower speed
    /// and a lower number results in a faster speed.
    /// </remarks>
    /// <value>The time, in milliseconds, between marque progress bar animation updates. Default value is 30.</value>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int MarqueeInterval
    {
        get => _marqueeInterval;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Must be greater than zero");
            }
            _marqueeInterval = value;
            RequestIntervalUpdate();
        }
    }

    /// <summary>Gets or sets the maximum progress bar value.</summary>
    /// <remarks>Default value is 100. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="OverflowException">The value is less than 0 or greater than 65535.</exception>
    /// <value>The minimum of the <see cref="Value"/> property.</value>
    public int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = checked((ushort)Math.Max(Value, value));
            RequestRangeUpdate();
        }
    }

    /// <summary>Gets or sets the minimum progress bar value.</summary>
    /// <remarks>Default value is 0. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="OverflowException">The value is less than 0 or greater than 65535.</exception>
    /// <value>The maximum of the <see cref="Value"/> property.</value>
    public int Minimum
    {
        get => _minimum;
        set
        {
            _minimum = checked((ushort)Math.Min(Value, value));
            RequestRangeUpdate();
            RequestValueUpdate(); // Needed to keep minimum synced.
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
    /// <remarks>
    /// When <see cref="Mode"/> is <see cref="ProgressBarMode.Marquee"/>, the progress bar will always take the appearance of
    /// <see cref="ProgressBarState.Normal"/>, as abnormal states are not supported by dialog marquee progress bars.
    /// </remarks>
    /// <value>The current state of the progress bar. Default value is <see cref="ProgressBarState.Normal"/>.</value>
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
    /// <remarks>When set, the value is clamped between <see cref="Minimum"/> and <see cref="Maximum"/> (inclusive).</remarks>
    /// <value>
    /// The position of the progress bar when <see cref="Mode"/> is <see cref="ProgressBarMode.Normal"/>. Default value is 0.
    /// </value>
    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, Minimum, Maximum);
            RequestValueUpdate();
            if (State is not ProgressBarState.Normal)
            {
                RequestValueUpdate(); // Needed to keep value synced when state is abnormal.
            }
        }
    }

    internal override void SetIn(in TASKDIALOGCONFIG container)
    {
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR, _mode is ProgressBarMode.Normal);
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee);
    }

    private protected override void InitializeState()
    {
        RequestModeUpdate();
        RequestRangeUpdate();
        RequestIntervalUpdate();
        RequestStateUpdate();
        RequestValueUpdate();
    }

    private void RequestIntervalUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_MARQUEE, true, (uint)_marqueeInterval));

    private void RequestModeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee));

    private void RequestRangeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_RANGE, 0, Macros.MAKELONG(_minimum, _maximum)));

    private void RequestStateUpdate() => RequestStateUpdate(_state);

    private void RequestStateUpdate(ProgressBarState state) => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_STATE, state));

    private void RequestValueUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_POS, _value));
}