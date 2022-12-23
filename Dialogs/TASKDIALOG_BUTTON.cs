using System.Runtime.InteropServices;

namespace Scover.Dialogs;

/// <summary>Temporary fixed version.</summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
internal struct TASKDIALOG_BUTTON
{
    public int nButtonID;
    public nint pszButtonText;
}