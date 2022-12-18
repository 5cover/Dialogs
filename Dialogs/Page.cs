using System.ComponentModel;
using System.Diagnostics;
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
public sealed partial class Page : IDisposable
{
    private readonly PartCollection _parts = new();
    private readonly Queue<Action<PageUpdateInfo>> _pendingUpdates = new();
    private bool _ignoreButtonClicked;
    private int _updateCount;

    /// <summary>Initializes a new empty <see cref="Page"/>.</summary>
    public Page()
    {
        Config.pfCallbackProc = Callback;
        _parts.PartAdded += (s, part) => part.UpdateRequested += Update;
        _parts.PartRemoved += (s, part) => part.UpdateRequested -= Update;

        void Update(object? sender, Action<PageUpdateInfo> update)
        {
            Trace.WriteLine($"Update #{++_updateCount}: {update.Method.Name}");
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
    /// <remarks>
    /// This event is raised before the <see cref="CommitControl.Clicked"/> event for individual commit controls. Set the <see
    /// cref="CancelEventArgs.Cancel"/> property of the event arguments to <see langword="true"/> to prevent the commit control
    /// from closing its containing page.
    /// </remarks>
    public event EventHandler<CommitControlClickedEventArgs>? ButtonClicked;
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
    /// <summary>Event raised when a button is clicked.</summary>
    /// <remarks>This event is raised before the <see cref="RadioButton.Clicked"/> event for individual radio buttons.</remarks>
    public event EventHandler<RadioButtonClickedEventArgs>? RadioButtonClicked;

    internal TASKDIALOGCONFIG Config { get; } = new();
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
        StringHelper.FreeString(Config.pszWindowTitle, CharSet.Unicode);
        StringHelper.FreeString(Config.pszMainInstruction, CharSet.Unicode);
        StringHelper.FreeString(Config.pszContent, CharSet.Unicode);
        StringHelper.FreeString(Config.pszVerificationText, CharSet.Unicode);
        StringHelper.FreeString(Config.pszExpandedControlText, CharSet.Unicode);
        StringHelper.FreeString(Config.pszCollapsedControlText, CharSet.Unicode);
        StringHelper.FreeString(Config.pszFooter, CharSet.Unicode);
        Buttons?.Dispose();
        Expander?.Dispose();
        RadioButtons?.Dispose();
    }

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
                CommitControlClickedEventArgs e = new(control);

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

            case TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED:
                var clickedRadioButton = RadioButtons!.GetControlFromId((int)wParam);
                Debug.Assert(clickedRadioButton is not null);
                RadioButtonClicked?.Invoke(this, new(clickedRadioButton));
                break;

            // For Help button, sent after TDN_BUTTON_CLICKED
            case TaskDialogNotification.TDN_HELP:
                HelpRequested.Raise(this);
                break;
        }

        return _parts.ForwardNotification(id, wParam, lParam);
    }

    private void SetElementText(TASKDIALOG_ELEMENTS element, nint pszText)
            => _ = OwnerDialog.SendMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, element, pszText);
}