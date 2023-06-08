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
    /// The inclusive upper bound of the range of IDs reserved for <see cref="CommonButton"/> instances (it's the
    /// maximum value of the <see cref="MB_RESULT"/> enumeration).
    /// </summary>
    internal const int MaxId = 11;

    /// <summary>
    /// The inclusive lower bound of the range of IDs reserved for <see cref="CommonButton"/> instances (it's the
    /// minimum value of the <see cref="MB_RESULT"/> enumeration).
    /// </summary>
    internal const int MinId = 1;

    private readonly TASKDIALOG_COMMON_BUTTON_FLAGS _commonButton;

    private readonly int _id;

    private CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, MB_RESULT id) : this(commonButton, (int)id)
    {
    }

    private CommonButton(TASKDIALOG_COMMON_BUTTON_FLAGS commonButton, int id)
    {
        Debug.Assert(MinId <= id && id <= MaxId);
        (_commonButton, _id) = (commonButton, id);
    }

    private CommonButton(int commonButton, MB_RESULT id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, (int)id)
    {
    }

    private CommonButton(int commonButton, int id) : this((TASKDIALOG_COMMON_BUTTON_FLAGS)commonButton, id)
    {
    }

    int IHasId.Id => _id;

    internal static IReadOnlyDictionary<string, CommonButton> Values { get; } = new Dictionary<string, CommonButton>()
    {
        [nameof(Button.Abort)] = new(1 << 16, IDABORT),
        [nameof(Button.Cancel)] = new(TDCBF_CANCEL_BUTTON, IDCANCEL),
        [nameof(Button.Close)] = new(TDCBF_CLOSE_BUTTON, 8),
        [nameof(Button.Continue)] = new(1 << 19, IDCONTINUE),
        [nameof(Button.Help)] = new(1 << 20, 9),
        [nameof(Button.Ignore)] = new(1 << 17, IDIGNORE),
        [nameof(Button.No)] = new(TDCBF_NO_BUTTON, IDNO),
        [nameof(Button.OK)] = new(TDCBF_OK_BUTTON, IDOK),
        [nameof(Button.Retry)] = new(TDCBF_RETRY_BUTTON, IDRETRY),
        [nameof(Button.TryAgain)] = new(1 << 18, IDTRYAGAIN),
        [nameof(Button.Yes)] = new(TDCBF_YES_BUTTON, IDYES),
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

    internal CommonButton CloneDeep() => new(_commonButton, _id);

    internal override void SetIn(in TASKDIALOGCONFIG config) => config.dwCommonButtons.SetFlag(_commonButton, true);
}