namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog push button controls. This class cannot be inherited. This class implements <see cref="IDisposable"/>.</summary>
public sealed class ButtonCollection : CommitControlCollection
{
    /// <summary>Adds a new push button to the collection.</summary>
    /// <param name="label">The label of the push button.</param>
    public void Add(string label) => Add(new Button(label));
}