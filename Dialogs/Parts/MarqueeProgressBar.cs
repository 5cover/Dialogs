using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog marquee progres bar control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class MarqueeProgressBar : ProgressBarBase
{
    private int _speed = 30;

    /// <summary>Gets or sets the speed.</summary>
    /// <value>The time, in milliseconds, between marque progress bar animation updates. Default value is 30.</value>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int Speed
    {
        get => _speed;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value is less than or equal to zero");
            }
            _speed = value;
            RequestSpeedUpdate();
        }
    }

    private protected override void InitializeState()
    {
        RequestSpeedUpdate();
        base.InitializeState();
    }

    private protected override void SetIn(in TASKDIALOGCONFIG container)
    {
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR, true);
        OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_MARQUEE_PROGRESS_BAR, true));
    }

    private void RequestSpeedUpdate()
        => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_PROGRESS_BAR_MARQUEE, true, Speed));
}