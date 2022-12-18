using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

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

/// <summary>A dialog control.</summary>
public abstract class DialogControl<TUpdateInfo>
{
    internal event EventHandler<Action<TUpdateInfo>>? UpdateRequested;

    /// <summary>Handles a notification as part of the Chain of Responsibility pattern.</summary>
    /// <remarks>
    /// Overrides must call the base method, as it handles state initialization. The base method always returns <see langword="default"/>.
    /// </remarks>
    /// <returns><see langword="default"/> if the notification wasn't handled, the notification-specific return value otherwise.</returns>
    internal virtual HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id == TaskDialogNotification.TDN_DIALOG_CONSTRUCTED)
        {
            InitializeState();
        }
        return default;
    }

    /// <summary>Layouts this object in a task dialog configuration object.</summary>
    /// <remarks>Overrides should not call base.</remarks>
    internal virtual void SetIn(in TASKDIALOGCONFIG config)
    {
    }

    /// <summary>Initializes state properties.</summary>
    /// <remarks>Overrides should not call base.</remarks>
    private protected virtual void InitializeState()
    {
    }

    /// <summary>Raises the <see cref="UpdateRequested"/> event.</summary>
    private protected void OnUpdateRequested(Action<TUpdateInfo> update) => UpdateRequested?.Invoke(this, update);
}