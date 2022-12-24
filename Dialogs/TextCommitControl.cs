using System.Diagnostics;
using Vanara.InteropServices;

namespace Scover.Dialogs;

/// <summary>A dialog commit control with text. Can represent a push button or a command link</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(_nativeText)}}}")]
public abstract class TextCommitControl : CommitControl, IEquatable<TextCommitControl?>, ITextControl, IDisposable
{
    private readonly SafeLPWSTR _nativeText = SafeLPWSTR.Null;

    private protected TextCommitControl(string text) => _nativeText = new(text);

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <inheritdoc/>
    public void Dispose()
    {
        _nativeText.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TextCommitControl);

    /// <inheritdoc/>
    public bool Equals(TextCommitControl? other) => other is not null && _nativeText.Equals(other._nativeText);

    /// <inheritdoc/>
    public override int GetHashCode() => _nativeText.GetHashCode();
}