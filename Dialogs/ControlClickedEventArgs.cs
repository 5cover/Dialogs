using System.ComponentModel;
using Scover.Dialogs.Parts;

namespace Scover.Dialogs;

/// <summary>Provides data for the an event about a click on a commit control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ControlClickedEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlClickedEventArgs"/> class with the <see
    /// cref="CancelEventArgs.Cancel"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedCotnrol">The commit control that initiated the closing of the dialog.</param>
    public ControlClickedEventArgs(CommitControl? clickedCotnrol) => ClickedControl = clickedCotnrol;

    /// <summary>Gets the commit control that initiated the closing of the dialog.</summary>
    public CommitControl? ClickedControl { get; set; }
}