namespace Scover.Dialogs;

/// <summary>Provides data for the an event about a click on a dialog radio button control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class RadioButtonClickedEventArgs : EventArgs
{
    /// <param name="clickedRadioButton">The radio buton that was clicked.</param>
    public RadioButtonClickedEventArgs(RadioButton clickedRadioButton) => ClickedRadioButton = clickedRadioButton;

    /// <summary>Gets a reference to the radio button that was clicked.</summary>
    public RadioButton ClickedRadioButton { get; set; }
}