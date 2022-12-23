using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

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