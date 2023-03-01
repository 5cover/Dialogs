using System.ComponentModel;

namespace Scover.Dialogs;

/// <summary>Provides data for the an event about a click on a dialog button.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class ButtonClickedEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonClickedEventArgs"/> class with the <see
    /// cref="CancelEventArgs.Cancel"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedCotnrol">The button that initiated the closing of the dialog.</param>
    public ButtonClickedEventArgs(ButtonBase clickedCotnrol) => ClickedButton = clickedCotnrol;

    /// <summary>Gets a reference to the button that initiated the closing of the dialog.</summary>
    public ButtonBase ClickedButton { get; set; }
}