using Vanara.InteropServices;

namespace Scover.Dialogs;

internal interface ITextControl
{
    /// <summary>Gets the <c>LPWSTR</c> that represents the text of this control.</summary>
    /// <remarks>The pointer is owned by the current object. Do not free it.</remarks>
    internal StrPtrUni NativeText { get; }
}
