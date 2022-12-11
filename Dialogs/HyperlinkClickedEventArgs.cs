namespace Scover.Dialogs;

/// <summary>Provides date for the <see cref="Page.HyperlinkClicked"/> event. This class cannot be inherited.</summary>
public sealed class HyperlinkClickedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="HyperlinkClickedEventArgs"/> class.</summary>
    /// <param name="href">The value of <c>href</c> attribute of the hyperlink.</param>
    public HyperlinkClickedEventArgs(string? href) => Href = href;

    /// <summary>Gets the value of the <c>href</c> attribute of the hyperlink.</summary>
    public string? Href { get; }
}