using Scover.Dialogs.Parts;

namespace Scover.Dialogs;

/// <summary>A result returned by the last page shown on a dialog.</summary>
/// <remarks>This record cannot be inherited.</remarks>
/// <param name="ClickedControl">The commit control that was clicked and closed the dialog.</param>
/// <param name="SelectedRadioButton">The selected radio button.</param>
public sealed record DialogResult(CommitControl? ClickedControl,
                                  RadioButton? SelectedRadioButton)
{
    /// <summary>Gets the commit control that was clicked and closed the dialog.</summary>
    /// <value>
    /// If the dialog was closed using Alt-F4, Escape, or the title bar's close button, <see langword="null"/>, unless the page
    /// has a <see cref="Button.Cancel"/> button.
    /// </value>
    public CommitControl? ClickedControl { get; init; } = ClickedControl;

    /// <summary>Gets the selected radio button.</summary>
    public RadioButton? SelectedRadioButton { get; init; } = SelectedRadioButton;
}