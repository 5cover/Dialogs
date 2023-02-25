using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A default radio button choice strategy.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class DefaultRadioButton
{
    private DefaultRadioButton(TASKDIALOG_FLAGS flags)
        => Flags = flags;

    /// <summary>The default radio button is the first element of the collection.</summary>
    public static DefaultRadioButton First { get; } = new(default);

    /// <summary>There is no default radio button.</summary>
    public static DefaultRadioButton None { get; } = new(TDF_NO_DEFAULT_RADIO_BUTTON);

    internal TASKDIALOG_FLAGS Flags { get; }
}