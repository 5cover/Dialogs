using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A default radio button choice strategy.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class DefaultRadioButton
{
    /// <param name="radioButton">The default radio button.</param>
    public DefaultRadioButton(RadioButton radioButton) : this(radioButton, default) { }

    private DefaultRadioButton(RadioButton? radioButton, TASKDIALOG_FLAGS flags)
        => (RadioButton, Flags) = (radioButton, flags);

    /// <summary>The default radio button is the first element of the collection.</summary>
    public static DefaultRadioButton First { get; } = new(null, default);

    /// <summary>There is no default radio button.</summary>
    public static DefaultRadioButton None { get; } = new(null, TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON);

    internal TASKDIALOG_FLAGS Flags { get; }

    internal RadioButton? RadioButton { get; }

    /// <summary>Creates a new <see cref="DefaultRadioButton"/> instance.</summary>
    /// <param name="radioButton">The default radio button.</param>
    public static implicit operator DefaultRadioButton(RadioButton radioButton) => new(radioButton);
}