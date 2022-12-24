using System.ComponentModel;

namespace Scover.Dialogs;

/// <summary>Provides data for the <see cref="Page.Closing"/> event.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ClosingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosingEventArgs"/> class with the <see cref="CancelEventArgs.Cancel"/>
    /// property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedCotnrol">
    /// The commit control that was clicked to close the page, or <see langword="null"/> if the the dialog window was closed
    /// using Alt-F4, Escape, or the title bar's close button
    /// </param>
    public ClosingEventArgs(CommitControl? clickedCotnrol) => ClickedControl = clickedCotnrol;

    /// <summary>
    /// Gets the commit control that was clicked to close the page, or <see langword="null"/> if the the dialog window was
    /// closed using Alt-F4, Escape, or the title bar's close button.
    /// </summary>
    public CommitControl? ClickedControl { get; set; }
}