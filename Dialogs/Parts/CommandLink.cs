using Vanara.InteropServices;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog command link button control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
public sealed class CommandLink : CommitControl, INativeProvider<StrPtrUni>, IDisposable
{
    private readonly SafeLPWSTR _text;

    /// <summary>Initializes the instance of the <see cref="CommandLink"/> class.</summary>
    /// <param name="label">The command link label.</param>
    /// <param name="note">The command link supplemental instruction.</param>
    public CommandLink(string label, string? note = null) => (Label, Note, _text) = (label, note, new(note is null ? label : $"{label}\n{note}"));

    /// <summary>Gets the label of this command link.</summary>
    public string Label { get; }

    /// <summary>Gets the note (supplemental instruction) of this command link.</summary>
    public string? Note { get; }

    /// <inheritdoc/>
    public void Dispose() => _text.Dispose();

    StrPtrUni INativeProvider<StrPtrUni>.GetNative() => _text;
}
