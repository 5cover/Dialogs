using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>Defines an object's ability to layout itself into <typeparamref name="TContainer"/>.</summary>
/// <typeparam name="TContainer">The type of container this object can layout itself into.</typeparam>
internal interface ILayoutProvider<TContainer>
{
    /// <summary>Adds this instance to <paramref name="container"/>.</summary>
    /// <remarks>If this instance is already in <paramref name="container"/>, no duplicate will be added.</remarks>
    internal void SetIn(in TContainer container);
}

/// <summary>Defines an object's ability to provide a native, possibly unmanaged representation of itself.</summary>
internal interface INativeProvider<TNative>
{
    /// <summary>Gets a native representation of this object.</summary>
    /// <remarks>It is owned by the parent object; do not dispose of it.</remarks>
    TNative GetNative();
}

/// <summary>Defines a contract for this object to recieve and interpret notifications.</summary>
internal interface INotificationHandler
{
    /// <summary>Optionally handles a notification.</summary>
    /// <returns><see langword="default"/> if the notification wasn't handled, the notification-specific return value otherwise.</returns>
    internal HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam);
}

/// <summary>Defines an object's ability to initialize its state when a dialog is first shown.</summary>
internal interface IStateInitializer
{
    /// <summary>
    /// Initializes this object's state, possibly by requesting updates using the <see cref="IUpdateRequester{TUpdate}"/> interface.
    /// </summary>
    internal void InitializeState();
}

/// <summary>Defines an object's ability to request updates when its state has changed.</summary>
/// <remarks>
/// Used when an object knows how and when to update itself but doesn't have enough information available to perform it.
/// </remarks>
/// <typeparam name="TUpdate">The type of information this object needs to perform its update.</typeparam>
internal interface IUpdateRequester<TUpdate>
{
    /// <summary>Event raised when an update has been requested.</summary>
    internal event EventHandler<Action<TUpdate>>? UpdateRequested;
}

/// <summary>A state update. This record cannot be inherited.</summary>
/// <param name="Dialog">The HWND of the dialog the update is occuring in.</param>
internal sealed record PageUpdate(HWND Dialog);

/// <summary>An state update with control ID information. This record cannot be inherited.</summary>
/// <param name="Dialog">The HWND of the dialog the update is occuring in.</param>
/// <param name="ControlId">The ID of the control involved.</param>
internal sealed record IdControlUpdate(HWND Dialog, int ControlId);