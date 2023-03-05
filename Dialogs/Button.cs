using System.Diagnostics;
using System.Runtime.CompilerServices;

using Vanara.InteropServices;

namespace Scover.Dialogs;

/// <summary>A dialog button with text. Can represent a push button or a command link.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(_nativeText)}}}")]
public sealed class Button : ButtonBase, IEquatable<Button?>, ITextControl, IDisposable
{
    private readonly SafeLPWSTR _nativeText = SafeLPWSTR.Null;

    /// <param name="text">The push button or command link text.</param>
    /// <param name="note">The command link supplemental instruction.</param>
    public Button(string text, string? note = null)
        => (Text, Note, _nativeText) = (text, note, new(note is null ? text : $"{text}\n{note}"));

    /// <summary>Gets a new <i>Abort</i> button.</summary>
    public static CommonButton Abort => GetValue();

    /// <summary>Gets a new <i>Cancel</i> button.</summary>
    /// <remarks>
    /// Similarly to <see cref="Page.IsCancelable"/>, adding this button to <see cref="Page.Buttons"/>
    /// causes the dialog window to respond to typical cancel actions (Alt-F4 and Escape) and have a close
    /// button on its title bar.
    /// </remarks>
    public static CommonButton Cancel => GetValue();

    /// <summary>Gets a new <i>Close</i> button.</summary>
    public static CommonButton Close => GetValue();

    /// <summary>Gets a new <i>Continue</i> button.</summary>
    public static CommonButton Continue => GetValue();

    /// <summary>Gets a new <i>Help</i> button.</summary>
    /// <remarks>
    /// This button is non-committing and raises the <see cref="Page.HelpRequested"/> event when clicked.
    /// </remarks>
    public static CommonButton Help => GetValue();

    /// <summary>Gets a new <i>Ignore</i> button.</summary>
    public static CommonButton Ignore => GetValue();

    /// <summary>Gets a new <i>No</i> button.</summary>
    public static CommonButton No => GetValue();

    /// <summary>Gets a new <i>OK</i> button.</summary>
    public static CommonButton OK => GetValue();

    /// <summary>Gets a new <i>Retry</i> button.</summary>
    public static CommonButton Retry => GetValue();

    /// <summary>Gets a new <i>Try Again</i> button.</summary>
    public static CommonButton TryAgain => GetValue();

    /// <summary>Gets a new <i>Yes</i> button.</summary>
    public static CommonButton Yes => GetValue();

    /// <summary>Gets the push button or command link text.</summary>
    public string Text { get; }

    /// <summary>Gets the command link supplemental instruction.</summary>
    /// <value>
    /// The supplemental instruction of this command link. If this button is not a command link or it
    /// doesn't have a supplemental insruction, <see langword="null"/>.
    /// </value>
    public string? Note { get; }

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <inheritdoc/>
    public void Dispose()
    {
        _nativeText.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Button);

    /// <inheritdoc/>
    public bool Equals(Button? other) => other is not null && _nativeText.Equals(other._nativeText);

    /// <inheritdoc/>
    public override int GetHashCode() => _nativeText.GetHashCode();

    private static CommonButton GetValue([CallerMemberName] string callerMemberName = "") => CommonButton.Values[callerMemberName].CloneDeep();
}