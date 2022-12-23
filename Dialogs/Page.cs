using System.ComponentModel;
using System.Runtime.InteropServices;
using Scover.Dialogs.Parts;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A page on a dialog.</summary>
/// <remarks>
/// This class cannot be inherited and implements <see cref="IDisposable"/>. When disposed, calls <see
/// cref="IDisposable.Dispose"/> on <see cref="Buttons"/>, <see cref="Expander"/> and <see cref="RadioButtons"/>.
/// </remarks>
public sealed partial class Page : IDisposable
{
    private readonly TASKDIALOGCONFIG _config = new();
    private readonly PartCollection _parts = new();
    private bool _handleSent;
    private bool _ignoreButtonClicked;

    /// <summary>Initializes a new empty <see cref="Page"/>.</summary>
    public Page()
    {
        _config.pfCallbackProc = Callback;

        _parts.SetDefaultValue(Sizing.Automatic);
        _parts.SetDefaultValue(DialogHeader.None);

        _parts.PartAdded += (s, part) => part.UpdateRequested += Update;
        _parts.PartRemoved += (s, part) => part.UpdateRequested -= Update;

        void Update(object? sender, Action<PageUpdateInfo> update) => OnUpdateRequested(update);
    }

    /// <summary>
    /// Event raised when the page is about to be closed, either because a commit control was clicked, or the dialog window was
    /// closed using Alt-F4, Escape, or the title bar's close button.Set the <see cref="CancelEventArgs.Cancel"/> property of
    /// the event arguments to <see langword="true"/> to prevent the page from closing.
    /// </summary>
    public event EventHandler<CommitControlClickedEventArgs>? Closing;
    /// <summary>Event raised when the page has been created.</summary>
    public event EventHandler? Created;
    /// <summary>Event raised when the page has been destroyed.</summary>
    public event EventHandler? Destroyed;
    /// <summary>
    /// Event raised when help was requested, either because the <see cref="Button.Help"/> button was clicked, or the F1 key was pressed.
    /// </summary>
    public event EventHandler? HelpRequested;
    /// <summary>Event raised when a hyperlink defined in the text areas of the dialog was clicked.</summary>
    public event EventHandler<HyperlinkClickedEventArgs>? HyperlinkClicked;
    internal event EventHandler<HWND>? HandleRecieved;
    internal event EventHandler<Action<PageUpdateInfo>>? UpdateRequested;

    /// <summary>Closes the page.</summary>
    public void Close()
    {
        _ignoreButtonClicked = true;
        OnUpdateRequested(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_BUTTON, Button.Cancel.Id));
        _ignoreButtonClicked = false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        StringHelper.FreeString(_config.pszWindowTitle, CharSet.Unicode);
        StringHelper.FreeString(_config.pszMainInstruction, CharSet.Unicode);
        StringHelper.FreeString(_config.pszContent, CharSet.Unicode);
        StringHelper.FreeString(_config.pszVerificationText, CharSet.Unicode);
        StringHelper.FreeString(_config.pszExpandedControlText, CharSet.Unicode);
        StringHelper.FreeString(_config.pszCollapsedControlText, CharSet.Unicode);
        StringHelper.FreeString(_config.pszFooter, CharSet.Unicode);
        Buttons?.Dispose();
        Expander?.Dispose();
        RadioButtons?.Dispose();
    }

    internal SafeHGlobalHandle CreateConfigPtr()
    {
        _parts.SetIn(_config);
        var ptr = _config.MarshalToPtr(Marshal.AllocHGlobal, out var bytesAllocated);
        return new(ptr, bytesAllocated);
    }

    internal CommitControl Show(HWND parent, WindowLocation startupLocation)
    {
        _parts.SetIn(_config);
        _config.hwndParent = parent;
        _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW, startupLocation is WindowLocation.CenterParent);
        TaskDialogIndirect(_config, out int pnButton, out _, out _).ThrowIfFailed();
        return Buttons?.GetControlFromId(pnButton) ?? Button.OK; // An OK Button is shown by default when there are no buttons defined.
    }

    private HRESULT Callback(HWND hwnd, TaskDialogNotification id, nint wParam, nint lParam, nint refData)
    {
        if (!_handleSent)
        {
            HandleRecieved?.Invoke(this, hwnd);
            _handleSent = true;
        }

        switch (id)
        {
            // Sent after TDN_DIALOG_CONSTRUCTED
            case TaskDialogNotification.TDN_CREATED:
                Created.Raise(this);
                break;

            case TaskDialogNotification.TDN_NAVIGATED:
                break;

            // Also sent when the dialog window was closed.
            case TaskDialogNotification.TDN_BUTTON_CLICKED:
                if (_ignoreButtonClicked)
                {
                    // stop handling now
                    return default;
                }
                var control = Buttons?.GetControlFromId((int)wParam) ?? Button.OK;
                // Check for the Help button because it is non-committing and natively doesnt close the dialog, so it wouldn't
                // make sense to raise Closing for it. If S_FALSE is returned for the Help button, TDN_HELP will not be sent.
                if (!control.Equals(Button.Help))
                {
                    CommitControlClickedEventArgs e = new(control);
                    Closing?.Invoke(this, e);
                    if (e.Cancel)
                    {
                        return HRESULT.S_FALSE;
                    }
                }
                break;

            case TaskDialogNotification.TDN_HYPERLINK_CLICKED:
                HyperlinkClicked?.Invoke(this, new(Marshal.PtrToStringUni(lParam)));
                break;

            case TaskDialogNotification.TDN_DESTROYED:
                Destroyed.Raise(this);
                break;

            case TaskDialogNotification.TDN_DIALOG_CONSTRUCTED:
                // Needed for reverting icon after header.
                OnUpdateRequested(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_UPDATE_ICON, TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_MAIN, Icon.Handle));
                break;

            // For Help button, sent after TDN_BUTTON_CLICKED
            case TaskDialogNotification.TDN_HELP:
                HelpRequested.Raise(this);
                break;
        }

        return _parts.Where(part => part is not null).Cast<DialogControl<PageUpdateInfo>>().ForwardNotification(new(id, wParam, lParam)) ?? default;
    }

    private void OnUpdateRequested(Action<PageUpdateInfo> update) => UpdateRequested?.Invoke(this, update);

    private void SetElementText(TASKDIALOG_ELEMENTS element, nint pszText)
        => OnUpdateRequested(info => info.Dialog.SendMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, element, pszText));
}