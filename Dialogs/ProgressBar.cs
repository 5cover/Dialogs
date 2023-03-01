using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>The mode of a dialog progress bar.</summary>
public enum ProgressBarMode
{
    /// <summary>
    /// Normal mode. The progress bar is indicated with a continuous bar that fills in from left to right.
    /// </summary>
    Normal,

    /// <summary>
    /// Marquee mode. The progress is indicated with a block that scrolls across the progress bar in a
    /// marquee fashion.
    /// </summary>
    Marquee
}

/// <summary>The state of a dialog progress bar control.</summary>
public enum ProgressBarState
{
    /// <summary>In progress.</summary>
    Normal = ProgressState.PBST_NORMAL,

    /// <summary>Paused.</summary>
    Paused = ProgressState.PBST_PAUSED,

    /// <summary>Interrupted due to an error.</summary>
    Error = ProgressState.PBST_ERROR
}

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
    /// Since the marquee animation interval is a time period, setting the value to a higher number results
    /// in a slower speed and a lower number results in a faster speed.
    /// </remarks>
    /// <value>
    /// The time, in milliseconds, between marque progress bar animation updates. Default value is 30.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int MarqueeInterval
    {
        get => _marqueeInterval;
        set
        {
            _marqueeInterval = value;
            RequestUpdate(UpdateInterval);
        }
    }

    /// <summary>Gets or sets the maximum progress bar value.</summary>
    /// <remarks>Default value is 100. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is less than 0 or greater than 65535.
    /// </exception>
    /// <value>The maximum of the <see cref="Value"/> property.</value>
    public int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = CheckAndConvertToUInt16(value);
            RequestUpdate(UpdateRange);
        }
    }

    /// <summary>Gets or sets the minimum progress bar value.</summary>
    /// <remarks>Default value is 0. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is less than 0 or greater than 65535.
    /// </exception>
    /// <value>The minimum of the <see cref="Value"/> property.</value>
    public int Minimum
    {
        get => _minimum;
        set
        {
            _minimum = CheckAndConvertToUInt16(value);
            RequestUpdate(info =>
            {
                UpdateRange(info);
                UpdateValue(info); // Needed to keep minimum synced.
            });
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
            RequestUpdate(info =>
            {
                if (_mode is ProgressBarMode.Marquee)
                {
                    // Set underlying state to normal -- Marquee doesn't support abnormal states.
                    UpdateState(info, ProgressBarState.Normal);
                }
                UpdateMode(info);
                if (_mode is ProgressBarMode.Normal)
                {
                    UpdateValue(info); // Needed for transitioning from Marquee to Normal
                    UpdateState(info); // Update the underlying state after leaving Marquee mode
                }
            });
        }
    }

    /// <summary>Gets or sets the progress bar state.</summary>
    /// <remarks>
    /// When <see cref="Mode"/> is <see cref="ProgressBarMode.Marquee"/>, the progress bar will always take
    /// the appearance of <see cref="ProgressBarState.Normal"/>, as abnormal states are not supported by
    /// dialog marquee progress bars.
    /// </remarks>
    /// <value>
    /// The current state of the progress bar. Default value is <see cref="ProgressBarState.Normal"/>.
    /// </value>
    public ProgressBarState State
    {
        get => _state;
        set
        {
            _state = value;
            if (Mode is ProgressBarMode.Normal) // Marquee doesn't support abnormal states.
            {
                RequestUpdate(UpdateState);
            }
        }
    }

    /// <summary>Gets or sets the current progress bar value.</summary>
    /// <remarks>
    /// When set, the value must be between <see cref="Minimum"/> and <see cref="Maximum"/> (inclusive).
    /// </remarks>
    /// <value>
    /// The position of the progress bar when <see cref="Mode"/> is <see cref="ProgressBarMode.Normal"/>.
    /// Default value is 0.
    /// </value>
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            RequestUpdate(UpdateValue);
        }
    }

    internal override void SetIn(in TASKDIALOGCONFIG container)
    {
        container.dwFlags.SetFlag(TDF_SHOW_PROGRESS_BAR, _mode is ProgressBarMode.Normal);
        container.dwFlags.SetFlag(TDF_SHOW_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee);
    }

    /// <inheritdoc/>
    protected override void InitializeState() => RequestUpdate(info =>
    {
        UpdateMode(info);
        UpdateRange(info);
        UpdateInterval(info);
        UpdateState(info);
        UpdateValue(info);
    });

    private static ushort CheckAndConvertToUInt16(int value)
    {
        if (value is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value,
                $"The value is less than {ushort.MinValue} or greater than {ushort.MaxValue}.");
        }
        return (ushort)value;
    }

    private static void UpdateState(PageUpdateInfo info, ProgressBarState state) => info.Dialog.SendMessage(TDM_SET_PROGRESS_BAR_STATE, state);

    private void UpdateInterval(PageUpdateInfo info) => info.Dialog.SendMessage(TDM_SET_PROGRESS_BAR_MARQUEE, true, (uint)_marqueeInterval);

    private void UpdateMode(PageUpdateInfo info) => info.Dialog.SendMessage(TDM_SET_MARQUEE_PROGRESS_BAR, _mode is ProgressBarMode.Marquee);

    private void UpdateRange(PageUpdateInfo info) => info.Dialog.SendMessage(TDM_SET_PROGRESS_BAR_RANGE, 0, Macros.MAKELONG(_minimum, _maximum));

    private void UpdateState(PageUpdateInfo info) => UpdateState(info, _state);

    private void UpdateValue(PageUpdateInfo info) => info.Dialog.SendMessage(TDM_SET_PROGRESS_BAR_POS, _value);
}