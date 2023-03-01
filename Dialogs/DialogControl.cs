﻿using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A dialog control.</summary>
public abstract class DialogControl<TUpdateInfo>
{
    internal event EventHandler<Action<TUpdateInfo>>? UpdateRequested;

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