using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A collection of <see cref="CommitControl"/> objects.</summary>
/// <remarks>This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.</remarks>
public abstract class CommitControlCollection : IdControlCollection<CommitControl>
{
    private protected CommitControlCollection(CommitControl? defaultItem) : base(defaultItem)
    {
    }

    internal override CommitControl? GetControlFromId(int id) => this.OfType<CommonButton>().SingleOrDefault(cb => cb.Id == id) ?? base.GetControlFromId(id);

    /// <remarks>
    /// <inheritdoc path="/remarks"/>
    /// <item>
    /// <term><see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/></term>
    /// <term>Forwards the notification to the clicked commit control.</term>
    /// <term><see cref="CommitControl.HandleNotification(Notification)"/></term>
    /// </item>
    /// </remarks>
    /// <returns>
    /// The notification is forwarded to all items. <see langword="null"/> if none of the items had a meaningful value to
    /// return, the notification-specific return value otherwise.
    /// </returns>
    /// <inheritdoc/>
    internal override HRESULT? HandleNotification(Notification notif)
    {
        _ = base.HandleNotification(notif);
        if (notif.Id is TaskDialogNotification.TDN_BUTTON_CLICKED)
        {
            return GetControlFromId((int)notif.WParam)?.HandleNotification(notif);
        }
        return this.ForwardNotification(notif);
    }

    private protected override int GetId(int index) => base.GetId(index) + CommonButton.MaxId;

    private protected override int GetIndex(int id) => base.GetIndex(id) - CommonButton.MaxId;

    private protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pButtons, config.cButtons, config.nDefaultButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}