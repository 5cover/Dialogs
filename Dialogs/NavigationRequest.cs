namespace Scover.Dialogs;

/// <summary>A request for a navigation operation.</summary>
public record NavigationRequest
{
    internal NavigationRequest(CommitControl? clickedControl, NavigationRequestKind kind)
        => (ClickedControl, Kind) = (clickedControl, kind);

    /// <summary>
    /// Gets the commit control that was clicked to exit the page or <see langword="null"/> if <see
    /// cref="Page.Exit()"/> was called.
    /// </summary>
    public CommitControl? ClickedControl { get; }

    /// <summary>Gets the kind of the navigation request.</summary>
    public NavigationRequestKind Kind { get; }
}
