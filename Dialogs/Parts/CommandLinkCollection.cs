using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog command link button controls. This class cannot be inherited.</summary>
public sealed class CommandLinkCollection : CommitControlCollection
{
    /// <summary>Gets or set whether a small arrow icon should be shown near the command links.</summary>
    /// <remarks>Default value is <see langword="true"/>.</remarks>
    public bool ShowArrowGlyph { get; set; } = true;

    private protected override TASKDIALOG_FLAGS Flags => ShowArrowGlyph ? TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS : TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS_NO_ICON;

    /// <summary>Adds a new command link to the collection.</summary>
    /// <param name="label">The label of the command link.</param>
    /// <param name="note">The supplemental instruction of the command link.</param>
    public void Add(string label, string? note = null) => Add(new Button(label, note));
}