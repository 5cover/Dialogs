using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of <see cref="CommitControl"/> objects.</summary>
public abstract class CommitControlCollection : IdControlCollection<CommitControl>, INotificationHandler
{
    private protected virtual TASKDIALOG_FLAGS Flags { get; }

    HRESULT INotificationHandler.HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_BUTTON_CLICKED && GetControlFromId((int)wParam) is INotificationHandler notificationHandler)
        {
            return notificationHandler.HandleNotification(id, wParam, lParam);
        }
        return default;
    }

    internal override CommitControl? GetControlFromId(int id) => CommonButton.FromId(id) ?? base.GetControlFromId(id);

    private protected override int GetId(int index) => index + 1 + CommonButton.MaxId;

    private protected override int GetIndex(int id) => id - 1 - CommonButton.MaxId;

    private protected override void SetContainerProperties(in TASKDIALOGCONFIG container, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
    {
        (container.pButtons, container.cButtons, container.nDefaultButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
        container.dwFlags |= Flags;
    }
}