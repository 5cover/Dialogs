using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

internal record struct Notification(TaskDialogNotification Id, nint WParam, nint LParam);

/// <summary>A dialog control.</summary>
public abstract class DialogControl<TUpdateInfo>
{
    internal event TypeEventHandler<DialogControl<TUpdateInfo>, Action<TUpdateInfo>>? UpdateRequested;

    /// <summary>Handles a notification.</summary>
    /// <remarks>
    /// Overrides must call base method before processing.
    /// <list type="table">
    /// <listheader>
    /// <term>Notification</term>
    /// <term>Behavior</term>
    /// <term>Return value</term>
    /// </listheader>
    /// <item>
    /// <term><see cref="TDN_CREATED"/> or <see cref="TDN_NAVIGATED"/></term>
    /// <term>Calls <see cref="InitializeState"/></term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// <see langword="default"/> if there was no meaningful value to return, the notification-specific
    /// return value otherwise.
    /// </returns>
    internal virtual HRESULT HandleNotification(Notification notif)
    {
        // TDN_DIALOG_CONSTRUCTED cannot be used here, even though it is recieved before TDN_CREATED and
        // TDN_NAVIGATED.
        if (notif.Id is TDN_CREATED or TDN_NAVIGATED)
        {
            InitializeState();
        }
        return default;
    }

    /// <summary>Layouts this object in a task dialog configuration object.</summary>
    /// <remarks>
    /// Overrides should not call base method defined in <see cref="DialogControl{TUpdateInfo}"/>.
    /// </remarks>
    internal virtual void SetIn(in TASKDIALOGCONFIG config)
    {
    }

    /// <summary>Initializes state properties.</summary>
    /// <remarks>
    /// Overrides should not call base method defined in <see cref="DialogControl{TUpdateInfo}"/>.
    /// </remarks>
    protected virtual void InitializeState()
    {
    }

    /// <summary>Raises the <see cref="UpdateRequested"/> event.</summary>
    protected void RequestUpdate(Action<TUpdateInfo> update) => UpdateRequested?.Invoke(this, update);
}