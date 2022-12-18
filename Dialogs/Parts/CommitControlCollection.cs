using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of <see cref="CommitControl"/> objects.</summary>
/// <remarks>This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.</remarks>
public abstract class CommitControlCollection : IdControlCollection<CommitControl>
{
    private protected CommitControlCollection(CommitControl? defaultItem) : base(defaultItem)
    {
    }

    internal override CommitControl? GetControlFromId(int id) => Items.OfType<CommonButton>().SingleOrDefault(cb => cb.Id == id) ?? base.GetControlFromId(id);

    internal override HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_BUTTON_CLICKED && GetControlFromId((int)wParam) is { } control)
        {
            return control.HandleNotification(id, wParam, lParam);
        }
        return base.HandleNotification(id, wParam, lParam);
    }

    private protected override int GetId(int index) => base.GetId(index) + CommonButton.MaxId;

    private protected override int GetIndex(int id) => base.GetIndex(id) - CommonButton.MaxId;

    private protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pButtons, config.cButtons, config.nDefaultButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}