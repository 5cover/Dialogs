using System.Diagnostics;

using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.User32.MB_RESULT;

namespace Scover.Dialogs;

/// <summary>A dialog button control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
[DebuggerDisplay($"{{{nameof(_commonButton)}}}")]
public sealed class CommonButton : ButtonBase, IEquatable<CommonButton?>, IHasId
{
    /// <summary>
    /// The upper bounds of the range of IDs reserved for <see cref="CommonButton"/> instances (it's the
    /// maximum value of the <see cref="MB_RESULT"/> enumeration).
    /// </summary>
    /// <remarks>The range is 0 to <see cref="MaxId"/>, inclusive.</remarks>
    internal const int MaxId = 11;

    private readonly TASKDIALOG_COMMON_BUTTON_FLAGS _commonButton;

    private readonly int _id;

    internal CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, MB_RESULT id) : this(commonButton, (int)id)
    {
    }

    internal CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, int id)
    {
        Debug.Assert(id <= MaxId);
        (_commonButton, _id) = (commonButton, id);
    }

    internal CommonButton(int commonButton, MB_RESULT id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, (int)id)
    {
    }

    internal CommonButton(int commonButton, int id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, id)
    {
    }

    int IHasId.Id => _id;

    internal static IReadOnlyDictionary<string, CommonButton> Values { get; } = new Dictionary<string, CommonButton>()
    {
        ["Abort"] = new(1 << 16, IDABORT),
        ["Cancel"] = new(TDCBF_CANCEL_BUTTON, IDCANCEL),
        ["Close"] = new(TDCBF_CLOSE_BUTTON, 8),
        ["Continue"] = new(1 << 19, IDCONTINUE),
        ["Help"] = new(1 << 20, 9),
        ["Ignore"] = new(1 << 17, IDIGNORE),
        ["No"] = new(TDCBF_NO_BUTTON, IDNO),
        ["OK"] = new(TDCBF_OK_BUTTON, IDOK),
        ["Retry"] = new(TDCBF_RETRY_BUTTON, IDRETRY),
        ["TryAgain"] = new(1 << 18, IDTRYAGAIN),
        ["Yes"] = new(TDCBF_YES_BUTTON, IDYES),
    };

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CommonButton);

    /// <inheritdoc/>
    public bool Equals(CommonButton? other) => other is not null && _id == other._id;

    /// <inheritdoc/>
    public override int GetHashCode() => _id.GetHashCode();

    /// <exception cref="ArgumentException"><paramref name="id"/> is an unknown common button ID.</exception>
    internal static CommonButton FromId(int id)
    {
        try
        {
            return Values.Values.Single(cb => cb._id == id).CloneDeep();
        }
        catch (InvalidOperationException e)
        {
            throw new ArgumentException("Unknown common button ID", nameof(id), e);
        }
    }

    internal override void SetIn(in TASKDIALOGCONFIG config) => config.dwCommonButtons.SetFlag(_commonButton, true);

    internal CommonButton CloneDeep() => new(_commonButton, _id);
}