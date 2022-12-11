using Scover.Dialogs.Parts;

namespace Scover.Dialogs;

/// <summary>A result returned by the last page shown on a dialog. This record cannot be inherited.</summary>
/// <param name="ClickedButton">The button that was clicked and closed the dialog.</param>
/// <param name="SelectedRadioButton">The radio button that was selected.</param>
public sealed record DialogResult(CommitControl? ClickedButton,
                                  RadioButton? SelectedRadioButton);