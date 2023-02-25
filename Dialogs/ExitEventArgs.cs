using System.ComponentModel;

namespace Scover.Dialogs;

/// <summary>Provides data for the <see cref="Page.Exiting"/> event.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ExitEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExitEventArgs"/> class with the <see
    /// cref="CancelEventArgs.Cancel"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedControl">The commit control that was clicked to exit the page.</param>
    public ExitEventArgs(CommitControl? clickedControl) => ClickedControl = clickedControl;

    /// <summary>
    /// Gets the commit control that was clicked to exit the page, or <see langword="null"/> if <see
    /// cref="Page.Exit"/> was called.
    /// </summary>
    public CommitControl? ClickedControl { get; set; }
}