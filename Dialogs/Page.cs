using System.ComponentModel;
using System.Runtime.InteropServices;

using Vanara.Extensions;
using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>Determines the position of a dialog window.</summary>
public enum WindowLocation
{
    /// <summary>The dialog window is placed at the center of the screen.</summary>
    CenterScreen,

    /// <summary>
    /// The dialog window is placed at the center of the parent window, or a the center of the screen if no
    /// parent window is specified.
    /// </summary>
    CenterParent
}

/// <summary>A page on a dialog.</summary>
/// <remarks>
/// This class cannot be inherited and implements <see cref="IDisposable"/>. When disposed, calls <see
/// cref="IDisposable.Dispose"/> on <see cref="Buttons"/>, <see cref="Expander"/> and <see
/// cref="RadioButtons"/>.
/// </remarks>
public partial class Page : IDisposable
{
    private readonly PartCollection _parts = new();
    private readonly TASKDIALOGCONFIG _config = new();
    private bool _disposed;

    /// <summary>Initializes a new empty <see cref="Page"/>.</summary>
    public Page()
    {
        _parts.PartAdded += (s, part) => part.UpdateRequested += OnUpdateRequested;
        _parts.PartRemoved += (s, part) => part.UpdateRequested -= OnUpdateRequested;

        _parts.RegisterDefaultValue(Sizing.Automatic);
        _parts.RegisterDefaultValue(DialogHeader.None);
        _parts.RegisterDefaultValue(new RadioButtonCollection());
        _parts.RegisterDefaultValue(new ButtonCollection());
    }

    /// <inheritdoc/>
    ~Page() => Dispose(disposing: false);

    /// <summary>
    /// Event raised when the page is about to be closed, either because a button was clicked, or the dialog
    /// window was closed. Set the <see cref="CancelEventArgs.Cancel"/> property of the event arguments to
    /// <see langword="true"/> to prevent the page from closing.
    /// </summary>
    public event EventHandler<ExitEventArgs>? Exiting;
    /// <summary>Event raised after the page has been created and before it is displayed.</summary>
    public event EventHandler? Created;
    /// <summary>Event raised when the dialog window is destroyed.</summary>
    public event EventHandler? Destroyed;
    /// <summary>
    /// Event raised when help was requested, either because the <see cref="Button.Help"/> button was
    /// clicked, or the F1 key was pressed.
    /// </summary>
    public event EventHandler? HelpRequested;
    /// <summary>Event raised when a hyperlink defined in the text areas of the dialog was clicked.</summary>
    public event EventHandler<HyperlinkClickedEventArgs>? HyperlinkClicked;
    internal event EventHandler<Action<PageUpdateInfo>>? UpdateRequested;
    internal bool Showing { get; set; }

    /// <summary>
    /// Exits the page. This may cause the dialog to close or a new page may be navigated to.
    /// </summary>
    /// <remarks>
    /// When this method is called, the return value of <see cref="Dialog.Show(nint?)"/> and the value of
    /// the <see cref="NavigationRequest.ClickedButton"/> property of the object passed into the <see
    /// cref="NextPageSelector"/> delegate provided to <see cref="MultiPageDialog"/> for navigation will be
    /// <see langword="null"/>.
    /// </remarks>
    public void Exit() => Exiting?.Invoke(this, new(null));

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    internal HRESULT HandleNotification(Notification notif)
    {
        switch (notif.Id)
        {
            // Also sent when the dialog window was closed.
            case TDN_BUTTON_CLICKED:
                return HandleClickNotification(notif);

            case TDN_HYPERLINK_CLICKED:
                HyperlinkClicked?.Invoke(this, new(Marshal.PtrToStringUni(notif.LParam)));
                break;

            case TDN_DESTROYED:
                Destroyed.Raise(this);
                break;

            // Sent before TDN_CREATED. TDN_CREATED is not sent after navigation.
            case TDN_DIALOG_CONSTRUCTED:
                // Needed for having an icon after the header was set.
                OnUpdateRequested(info => info.Dialog.SendMessage(TDM_UPDATE_ICON, TDIE_ICON_MAIN, Icon.Handle));
                Created.Raise(this);
                break;

            // For Help button, sent after TDN_BUTTON_CLICKED, unless S_FALSE was returned.
            case TDN_HELP:
                HelpRequested.Raise(this);
                break;
        }
        return _parts.ForwardNotification(notif);
    }

    internal TASKDIALOGCONFIG SetupConfig(TaskDialogCallbackProc callback)
    {
        _config.pfCallbackProc = callback;
        _parts.SetPartsIn(_config);
        return _config;
    }

    internal TASKDIALOGCONFIG SetupConfig(TaskDialogCallbackProc callback, HWND parent, WindowLocation startupLocation)
    {
        _ = SetupConfig(callback);
        _config.hwndParent = parent;
        _config.dwFlags.SetFlag(TDF_POSITION_RELATIVE_TO_WINDOW, startupLocation is WindowLocation.CenterParent);
        return _config;
    }

    internal ButtonBase? GetClickedButton(int pnButton) => Buttons.GetControlFromId(pnButton) ?? CommonButton.FromId(pnButton);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            Buttons.Dispose();
            Expander?.Dispose();
            RadioButtons.Dispose();
        }
        foreach (var p in new nint[]
        {
            _config.pszFooter,
            _config.pszContent,
            _config.pszWindowTitle,
            _config.pszMainInstruction,
            _config.pszVerificationText,
            _config.pszExpandedControlText,
            _config.pszCollapsedControlText,
        })
        {
            StringHelper.FreeString(p, CharSet.Unicode);
        }
        _disposed = true;
    }

    private HRESULT HandleClickNotification(Notification notif)
    {
        HRESULT buttonsResult = Buttons.HandleNotification(notif);
        if (buttonsResult != default)
        {
            return buttonsResult;
        }

        var button = GetClickedButton((int)notif.WParam);
        // Do not raise Closing for a non-committing button.
        if (IsCommitting(button))
        {
            ExitEventArgs e = new(button);
            Exiting?.Invoke(this, e);
            if (e.Cancel)
            {
                return HRESULT.S_FALSE;
            }
        }

        return default;

        // The Help button is non-committing as it doesn't close the dialog.
        static bool IsCommitting(ButtonBase? button) => !Button.Help.Equals(button);
    }

    private void OnUpdateRequested(object? sender, Action<PageUpdateInfo> update) => OnUpdateRequested(update);

    private void OnUpdateRequested(Action<PageUpdateInfo> update) => UpdateRequested?.Invoke(this, update);

    private void SetElementText(TASKDIALOG_ELEMENTS element, nint pszText)
        => OnUpdateRequested(info => info.Dialog.SendMessage(TDM_SET_ELEMENT_TEXT, element, pszText));
}