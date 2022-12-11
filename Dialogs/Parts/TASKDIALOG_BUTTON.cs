using System.Runtime.InteropServices;

namespace Scover.Dialogs.Parts;

/// <summary>Temporary fixed version.</summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
internal struct TASKDIALOG_BUTTON
{
    public int nButtonID;
    public IntPtr pszButtonText;
}