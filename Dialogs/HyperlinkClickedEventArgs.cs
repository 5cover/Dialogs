namespace Scover.Dialogs;

/// <summary>Provides date for the <see cref="Page.HyperlinkClicked"/> event.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class HyperlinkClickedEventArgs : EventArgs
{
    /// <param name="href">The value of the <c>href</c> attribute of the hyperlink.</param>
    public HyperlinkClickedEventArgs(string? href) => Href = href;

    /// <summary>Gets the value of the <c>href</c> attribute of the hyperlink.</summary>
    public string? Href { get; }
}