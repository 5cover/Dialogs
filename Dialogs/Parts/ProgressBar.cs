using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog progress bar control.</summary>
public sealed class ProgressBar : ProgressBarBase
{
    private const ushort DefaultMaximum = 100;
    private const ushort DefaultMinimum = 0;
    private const ushort DefaultValue = 0;
    private ushort _maximum = DefaultMaximum;
    private ushort _minimum = DefaultMinimum;
    private int _value = DefaultValue;

    /// <summary>Gets or sets the maximum progress bar value.</summary>
    /// <remarks>
    /// The minimum value is <see cref="ushort.MinValue"/>. The maximum value is <see cref="ushort.MaxValue"/>. Default value is 100.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is less than <see cref="ushort.MinValue"/> or greater than <see cref="ushort.MaxValue"/>.
    /// </exception>
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (value is < ushort.MinValue or > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The value is less than {ushort.MinValue} or greater than {ushort.MaxValue}.");
            }
            if (_maximum != value)
            {
                _maximum = (ushort)value;
                RequestRangeUpdate();
            }
        }
    }

    /// <summary>Gets or sets the _minimum progress bar value.</summary>
    /// <remarks>
    /// The minimum value is <see cref="ushort.MinValue"/>. The maximum value is <see cref="ushort.MaxValue"/>. Default value is 0.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is less than <see cref="ushort.MinValue"/> or greater than <see cref="ushort.MaxValue"/>.
    /// </exception>
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (value is < ushort.MinValue or > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The value is less than {ushort.MinValue} or greater than {ushort.MaxValue}.");
            }
            if (_minimum != value)
            {
                _minimum = (ushort)value;
                RequestRangeUpdate();
            }
        }
    }

    /// <summary>Gets or sets the current progress bar value.</summary>
    /// <remarks>Default value is 0.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is less than <see cref="Minimum"/> or greater than <see cref="Maximum"/>.
    /// </exception>
    public int Value
    {
        get => _value;
        set
        {
            if (value < Minimum || value > Maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The value is less than {nameof(Minimum)} ({Minimum}) or greater than {nameof(Maximum)} ({Maximum})");
            }
            if (value != Value)
            {
                _value = value;
                RequestPosUpdate();
            }
        }
    }

    private protected override void InitializeState()
    {
        if (Value != DefaultValue)
        {
            RequestPosUpdate();
        }
        if (Minimum != DefaultMinimum || Maximum != DefaultMaximum)
        {
            RequestRangeUpdate();
        }
    }

    private protected override void SetIn(in TASKDIALOGCONFIG container) => container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR, true);

    private void RequestPosUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_POS, _value));

    private void RequestRangeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_RANGE, Macros.MAKELONG(_minimum, _maximum)));
}