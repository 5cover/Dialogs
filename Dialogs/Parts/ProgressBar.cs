using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog progress bar control.</summary>
public sealed class ProgressBar : ProgressBarBase
{
    private ushort _maximum = 100;
    private ushort _minimum;
    private ushort _value;

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

    /// <summary>Gets or sets the current progress bar value.</summary>
    /// <remarks>Default value is 0. The value must be greater than 0 and less than 65535.</remarks>
    /// <exception cref="OverflowException">The value is less than 0 or greater than 65535.</exception>
    public int Value
    {
        get => _value;
        set
        {
            _value = checked((ushort)value);
            RequestPosUpdate();
        }
    }

    private protected override void InitializeState()
    {
        RequestPosUpdate();
        RequestRangeUpdate();
        base.InitializeState();
    }

    private protected override void SetIn(in TASKDIALOGCONFIG container) => container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR, true);

    private void RequestPosUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_POS, _value));

    private void RequestRangeUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_RANGE, lParam: Macros.MAKELONG(_minimum, _maximum)));
}