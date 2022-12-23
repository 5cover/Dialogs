using System.Diagnostics;
using Vanara.InteropServices;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog commit control with text.</summary>
/// <remarks>This class implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(_nativeText)}}}")]
public abstract class TextCommitControl : CommitControl, ITextControl, IDisposable
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
}