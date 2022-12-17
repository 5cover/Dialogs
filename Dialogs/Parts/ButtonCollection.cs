namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog push button controls. This class cannot be inherited.</summary>
/// <inheritdoc path="/remarks"/>
public sealed class ButtonCollection : CommitControlCollection
{
    /// <summary>Initializes a new instance of the <see cref="ButtonCollection"/> class.</summary>
    /// <remarks>The default button will be the first item of the collection.</remarks>
    public ButtonCollection() : base(null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ButtonCollection"/> class.</summary>
    /// <param name="defaultItem">
    /// The default button. If <see langword="null"/>, the default button will be the first item of the collection.
    /// </param>
    public ButtonCollection(Button? defaultItem) : base(defaultItem)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ButtonCollection"/> class.</summary>
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