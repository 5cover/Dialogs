namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog push button controls. This class cannot be inherited.</summary>
/// <remarks>
/// This class cannot be inherited and implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.
/// </remarks>
public sealed class ButtonCollection : CommitControlCollection
{
    /// <remarks>The default button will be the first item of the collection.</remarks>
    public ButtonCollection() : base(null)
    {
    }

    /// <param name="defaultItem">
    /// The default button. If <see langword="null"/>, the default button will be the first item of the collection.
    /// </param>
    public ButtonCollection(Button? defaultItem) : base(defaultItem)
    {
    }

    /// <param name="defaultItem">
    /// The default button. If <see langword="null"/>, the default button will be the first item of the collection.
    /// </param>
    public ButtonCollection(CommonButton? defaultItem) : base(defaultItem)
    {
    }

    /// <summary>Adds a new push button to the collection.</summary>
    /// <param name="label">The label of the push button.</param>
    public void Add(string label) => Add(new Button(label));
}