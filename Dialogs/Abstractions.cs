using Vanara.InteropServices;
using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>
/// A delegate that selects the next page to navigate to after a page closing or an explicit navigation
/// request.
/// </summary>
/// <param name="clickedControl">
/// The commit control that closed the previous page, or <see langword="null"/> if navigation was explicitly
/// requested with <see cref="MultiPageDialog.Navigate()"/>.
/// </param>
/// <returns>The next page to navigate to, or <see langword="null"/> to end the navigation.</returns>
public delegate Page? NextPageSelector(CommitControl? clickedControl);

internal interface ITextControl
{
    /// <summary>Gets the <c>LPWSTR</c> that represents the text of this control.</summary>
    /// <remarks>The pointer is owned by the current object. Do not free it.</remarks>
    internal StrPtrUni NativeText { get; }
}

/// <summary>ID control update information.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class IdControlUpdateInfo
{
    internal IdControlUpdateInfo(HWND dialog, int controlId) => (Dialog, ControlId) = (dialog, controlId);

    internal int ControlId { get; }
    internal HWND Dialog { get; }
}

/// <summary>Page update information.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class PageUpdateInfo
{
    internal PageUpdateInfo(HWND dialog) => Dialog = dialog;

    internal HWND Dialog { get; }
}

internal sealed record Notification(TaskDialogNotification Id, nint WParam, nint LParam);

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
    /// <term><see cref="TaskDialogNotification.TDN_DIALOG_CONSTRUCTED"/></term>
    /// <term>Calls <see cref="InitializeState"/></term>
    /// <term><see langword="null"/></term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// <see langword="null"/> if there was no meaningful value to return, the notification-specific return
    /// value otherwise.
    /// </returns>
    internal virtual HRESULT? HandleNotification(Notification notif)
    {
        if (notif.Id == TaskDialogNotification.TDN_DIALOG_CONSTRUCTED)
        {
            InitializeState();
        }
        return null;
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