using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

internal interface ILayoutProvider<TContainer>
{
    internal void SetIn(in TContainer container);
}

internal interface INativeProvider<TNative>
{
    TNative GetNative();
}

internal interface INotificationHandler
{
    internal HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam);
}

internal interface IStateInitializer
{
    internal void InitializeState();
}

internal interface IUpdateRequester<TUpdate>
{
    internal event EventHandler<Action<TUpdate>>? UpdateRequested;
}

internal sealed record PageUpdate(HWND Dialog);
internal sealed record IdControlUpdate(HWND Dialog, int ControlId);