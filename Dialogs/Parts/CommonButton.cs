using System.Diagnostics;
using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog button control. This class cannot be inherited.</summary>
public sealed class CommonButton : CommitControl, ILayoutProvider<TASKDIALOGCONFIG>, IEquatable<CommonButton?>
{
    /// <summary>The upper bounds of the range of IDs reserved for <see cref="CommonButton"/> instances.</summary>
    /// <remarks>The range is [0 ; <see cref="MaxId"/>].</remarks>
    internal const int MaxId = 11;

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

    /// <summary>Gets the id of this common button.</summary>
    internal int Id { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CommonButton);

    /// <inheritdoc/>
    public bool Equals(CommonButton? other) => other is not null && Id == other.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container) => container.dwCommonButtons.SetFlag(_commonButton, true);
}