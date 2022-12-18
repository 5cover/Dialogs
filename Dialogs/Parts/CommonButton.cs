using System.Diagnostics;
using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog button control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class CommonButton : CommitControl, IEquatable<CommonButton?>
{
    /// <summary>The upper bounds of the range of IDs reserved for <see cref="CommonButton"/> instances.</summary>
    /// <remarks>The range is 0 to <see cref="MaxId"/>, inclusive.</remarks>
    internal const int MaxId = 11;

    private static readonly CommonButton[] _values =
    {
        Button.Abort,
        Button.Cancel,
        Button.Close,
        Button.Continue,
        Button.Help,
        Button.Ignore,
        Button.No,
        Button.OK,
        Button.Retry,
        Button.TryAgain,
        Button.Yes,
    };

    private readonly TASKDIALOG_COMMON_BUTTON_FLAGS _commonButton;

    internal CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, MB_RESULT id) : this(commonButton, (int)id)
    {
    }

    internal CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, int id)
    {
        Debug.Assert(id <= MaxId);
        (_commonButton, Id) = (commonButton, id);
    }

    internal CommonButton(int commonButton, MB_RESULT id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, (int)id)
    {
    }

    internal CommonButton(int commonButton, int id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, id)
    {
    }

    internal int Id { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CommonButton);

    /// <inheritdoc/>
    public bool Equals(CommonButton? other) => other is not null && Id == other.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    internal static CommonButton? FromId(int id) => _values.SingleOrDefault(cb => cb.Id == id);

    internal override void SetIn(in TASKDIALOGCONFIG config) => config.dwCommonButtons.SetFlag(_commonButton, true);
}