namespace Scover.Dialogs;

/// <summary>Determines the position of a dialog window.</summary>
public enum WindowLocation
{
    /// <summary>The dialog window is placed at the center of the screen.</summary>
    CenterScreen,

    /// <summary>
    /// The dialog window is placed at the center of the parent window, or a the center of the screen if no parent window is specified.
    /// </summary>
    CenterParent
}