namespace Scover.Dialogs;

/// <summary>A request for a navigation operation.</summary>
public record NavigationRequest
{
    internal NavigationRequest(ButtonBase? clickedButton, NavigationRequestKind kind)
        => (ClickedButton, Kind) = (clickedButton, kind);

    /// <summary>
    /// Gets the button that was clicked to exit the page or <see langword="null"/> if <see
    /// cref="Page.Exit()"/> was called.
    /// </summary>
    public ButtonBase? ClickedButton { get; }

    /// <summary>Gets the kind of the navigation request.</summary>
    public NavigationRequestKind Kind { get; }
}