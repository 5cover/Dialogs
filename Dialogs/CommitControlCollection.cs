using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A style of commit controls.</summary>
public enum CommitControlStyle
{
    /// <summary>Commit controls are shown as push buttons.</summary>
    PushButtons,

    /// <summary>Commit controls are show as command link buttons.</summary>
    CommandLinks,

    /// <summary>Commit controls are shown as command link buttons without the arrow icon.</summary>
    CommandLinksNoIcon
}

/// <summary>A collection of <see cref="CommitControl"/> objects.</summary>
/// <remarks>
/// This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its
/// items.
/// </remarks>
public sealed class CommitControlCollection : IdControlCollection<CommitControl>
{
    /// <param name="defaultItem">
    /// The default button. If <see langword="null"/>, the default button will be the first item of the
    /// collection.
    /// </param>
    /// <param name="style">The commit control style to use.</param>
    /// <param name="items">The items already in the collection.</param>
    public CommitControlCollection(CommitControlStyle style = CommitControlStyle.PushButtons, IList<CommitControl>? items = null, CommitControl? defaultItem = null) : base(items, defaultItem) => Style = style;

    /// <summary>Gets or sets the commit control style to use for this collection.</summary>
    public CommitControlStyle Style { get; set; }

    ///<inheritdoc/>
    protected override TASKDIALOG_FLAGS Flags => Style switch
    {
        CommitControlStyle.PushButtons => default,
        CommitControlStyle.CommandLinks => TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS,
        CommitControlStyle.CommandLinksNoIcon => TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS_NO_ICON,
        _ => throw Style.InvalidEnumArgumentException()
    };

    /// <summary>Adds a new commit control to the collection.</summary>
    /// <param name="text">The push button text or the command link label.</param>
    /// <param name="note">The command link supplemental instruction.</param>
    public void Add(string text, string? note = null) => Add(new CommandLink(text, note));

    /// <summary>Adds a new push button to the collection.</summary>
    /// <param name="button">The button to add.</param>
    public void Add(CommonButton button) => base.Add(button.CloneDeep());

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
    /// The notification is forwarded to all items. <see langword="null"/> if none of the items had a
    /// meaningful value to return, the notification-specific return value otherwise.
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

    /// <inheritdoc/>
    protected override int GetId(int index) => base.GetId(index) + CommonButton.MaxId;

    /// <inheritdoc/>
    protected override int GetIndex(int id) => base.GetIndex(id) - CommonButton.MaxId;

    /// <inheritdoc/>
    protected override void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId)
        => (config.pButtons, config.cButtons, config.nDefaultButton) = (nativeButtonArrayHandle, nativeButtonArrayCount, defaultItemId);
}