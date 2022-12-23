namespace Scover.Dialogs.Parts;

/// <summary>A dialog command link button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
public sealed class CommandLink : TextCommitControl
{
    /// <param name="label">The command link label.</param>
    /// <param name="note">
    /// The command link supplemental instruction. If <see langword="null"/>, there will be no supplemental instruction area.
    /// </param>
    public CommandLink(string label, string? note = null) : base(note is null ? label : $"{label}\n{note}")
        => (Label, Note) = (label, note);

    /// <summary>Gets the label of this command link.</summary>
    public string Label { get; }

    /// <summary>Gets the note (supplemental instruction) of this command link.</summary>
    public string? Note { get; }
}