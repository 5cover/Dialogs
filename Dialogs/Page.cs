using System.Runtime.InteropServices;
using Scover.Dialogs.Parts;
using Vanara.Extensions;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A page on a dialog.</summary>
/// <remarks>
/// This class cannot be inherited and implements <see cref="IDisposable"/>. When disposed, calls <see
/// cref="IDisposable.Dispose"/> on <see cref="Buttons"/>, <see cref="Expander"/> and <see cref="RadioButtons"/>.
/// </remarks>
public sealed partial class Page : INativeProvider<TASKDIALOGCONFIG>, IDisposable
{
    private readonly PartCollection _parts = new();
    private readonly Queue<Action<PageUpdate>> _pendingUpdates = new();
    private readonly TASKDIALOGCONFIG _wrap = new();
    private bool _ignoreButtonClicked;

    /// <summary>Initializes a new instance of the <see cref="Page"/> class.</summary>
    public Page()
    {
        _wrap.pfCallbackProc = Callback;
        _parts.PartAdded += (s, part) =>
        {
            if (part is IUpdateRequester<PageUpdate> ur)
            {
                ur.UpdateRequested += Update;
            }
        };
        _parts.PartRemoved += (s, part) =>
        {
            if (part is IUpdateRequester<PageUpdate> ur)
            {
                ur.UpdateRequested -= Update;
            }
        };

        void Update(object? sender, Action<PageUpdate> update)
        {
            if (OwnerDialog.IsNull)
            {
                _pendingUpdates.Enqueue(update);
            }
            else
            {
                update(new(OwnerDialog));
            }
        }
    }

    /// <summary>Event raised when a button is clicked.</summary>
    public event EventHandler<ControlClickedEventArgs>? ButtonClicked;
    /// <summary>
    /// Event raised when the page is about to be closed, either because a commit control was clicked, or the dialog window was
    /// closed using Alt-F4, Escape, or the title bar's close button.
    /// </summary>
    public event EventHandler<ControlClickedEventArgs>? Closing;
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

    internal HWND OwnerDialog { get; private set; }

    /// <summary>Closes the page.</summary>
    public void Close()
    {
        _ignoreButtonClicked = true;
        _ = OwnerDialog.SendMessage(TaskDialogMessage.TDM_CLICK_BUTTON, Button.Cancel.Id);
        _ignoreButtonClicked = false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        StringHelper.FreeString(_wrap.pszWindowTitle, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszMainInstruction, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszContent, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszVerificationText, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszExpandedControlText, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszCollapsedControlText, CharSet.Unicode);
        StringHelper.FreeString(_wrap.pszFooter, CharSet.Unicode);
        Buttons?.Dispose();
        Expander?.Dispose();
        RadioButtons?.Dispose();
    }

    TASKDIALOGCONFIG INativeProvider<TASKDIALOGCONFIG>.GetNative() => _wrap;

    internal DialogResult GetResult(int clickedButtonId, int clickedRadioButtonId)
        => new(Buttons?.GetControlFromId(clickedButtonId), RadioButtons?.GetControlFromId(clickedRadioButtonId));

    private HRESULT Callback(HWND hwnd, TaskDialogNotification id, nint wParam, nint lParam, nint refData)
    {
        OwnerDialog = hwnd;

        if (!hwnd.IsNull)
        {
            while (_pendingUpdates.Count > 0)
            {
                _pendingUpdates.Dequeue()(new(hwnd));
            }
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

                var control = Buttons?.GetControlFromId((int)wParam);
                ControlClickedEventArgs e = new(control);

                ButtonClicked?.Invoke(this, e);
                if (e.Cancel)
                {
                    return HRESULT.S_FALSE;
                }
                // Check for the Help button because it is non-committing and natively doesnt close the dialog, so it wouldn't
                // make sense to raise Closing for it. Worse, if S_FALSE is returned for the Help button, TDN_HELP will not be sent.
                if (!Button.Help.Equals(control))
                {
                    // Reuse the event args. e.Cancel is false because we didn't return.
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
                foreach (var stateInitializer in _parts.GetParts<IStateInitializer>())
                {
                    stateInitializer.InitializeState();
                }
                break;

            // For Help button, sent after TDN_BUTTON_CLICKED
            case TaskDialogNotification.TDN_HELP:
                HelpRequested.Raise(this);
                break;
        }

        return _parts.GetParts<INotificationHandler>().Select(sr => sr.HandleNotification(id, wParam, lParam)).SingleOrDefault(h => h != default);
    }

    private void SetElementText(TASKDIALOG_ELEMENTS element, nint pszText)
            => _ = OwnerDialog.SendMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, element, pszText);
}