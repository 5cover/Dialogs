using System.ComponentModel;
using Scover.Dialogs.Parts;

namespace Scover.Dialogs;

/// <summary>Provides data for the an event about a click on a dialog commit control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class CommitControlClickedEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommitControlClickedEventArgs"/> class with the <see
    /// cref="CancelEventArgs.Cancel"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedCotnrol">The commit control that initiated the closing of the dialog.</param>
    public CommitControlClickedEventArgs(CommitControl clickedCotnrol) => ClickedControl = clickedCotnrol;

    /// <summary>Gets a reference to the commit control that initiated the closing of the dialog.</summary>
    public CommitControl ClickedControl { get; set; }
}