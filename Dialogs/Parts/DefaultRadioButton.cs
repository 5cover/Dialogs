using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A default radio button choice strategy.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class DefaultRadioButton
{
    private DefaultRadioButton(RadioButton? radioButton, TASKDIALOG_FLAGS flags)
            => (RadioButton, Flags) = (radioButton, flags);

    /// <summary>The default radio button is the first element of the collection.</summary>
    public static DefaultRadioButton First { get; } = new(null, default);

    /// <summary>There is no default radio button.</summary>
    public static DefaultRadioButton None { get; } = new(null, TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON);

    internal TASKDIALOG_FLAGS Flags { get; }

    internal RadioButton? RadioButton { get; }

    internal static DefaultRadioButton FromRadioButton(RadioButton radioButton) => new(radioButton, default);
}