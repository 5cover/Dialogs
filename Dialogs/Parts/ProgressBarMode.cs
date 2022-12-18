namespace Scover.Dialogs.Parts;

/*var progressBar = User32.FindWindowEx(Handle, HWND.NULL, "PROGRESS_CLASS", null);
User32.GetWindowRect(progressBar, out RECT progressBarRect);
return progressBarRect.Width;*/

/// <summary>The mode of a dialog progress bar.</summary>
public enum ProgressBarMode
{
    /// <summary>Normal mode. The progress bar is indicated with a continuous bar that fills in from left to right.</summary>
    Normal,

    /// <summary>Marquee mode. The progress is indicated with a block that scrolls across the progress bar in a marquee fashion.</summary>
    Marquee
}