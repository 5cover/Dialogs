namespace Scover.Dialogs;

/// <summary>The kind of a navigation request.</summary>
public enum NavigationRequestKind
{
    /// <summary>
    /// Navigation was explicitly requested by calling <see cref="MultiPageDialog.Navigate()"/>
    /// </summary>
    Explicit,

    /// <summary>
    /// Navigation was requested since the dialog window was closed or <see cref="Button.Cancel"/> button
    /// was clicked.
    /// </summary>
    Cancel,

    /// <summary>Navigation was requested since <see cref="Page.Exit()"/> was called.</summary>
    Exit,

    /// <summary>Navigation was requested since a commit control was clicked.</summary>
    Commit,
}
