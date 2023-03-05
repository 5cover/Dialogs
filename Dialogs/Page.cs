using System.ComponentModel;
using System.Runtime.InteropServices;

using Vanara.Extensions;
using Vanara.InteropServices;
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
    internal bool IsShown;
    private readonly PartCollection _parts = new();
    private readonly SafeLPWSTR _windowTitle = new((string?)null);
    private SafeLPWSTR _footerText = new((string?)null), _content = new((string?)null), _mainInstruction = new((string?)null);

    /// <summary>Initializes a new empty <see cref="Page"/>.</summary>
    public Page()
    {
        _parts.PartAdded += (s, part) => part.UpdateRequested += OnUpdateRequested;
        _parts.PartRemoved += (s, part) => part.UpdateRequested -= OnUpdateRequested;

        _parts.SetDefault(Sizing.Automatic, nameof(Sizing));
        _parts.SetDefault(DialogHeader.None, nameof(Header));
        _parts.SetDefault(new ButtonCollection(), nameof(Buttons));
        _parts.SetDefault(new RadioButtonCollection(), nameof(RadioButtons));
    }

    /// <summary>
    /// Event raised when the page is about to be closed, either because a button was clicked, or the dialog
    /// window was closed. Set the <see cref="CancelEventArgs.Cancel"/> property of the event arguments to
    /// <see langword="true"/> to prevent the page from closing.
    /// </summary>
    public event EventHandler<ExitEventArgs>? Exiting;

    /// <summary>Event raised when the page is shown.</summary>
    public event EventHandler? Created;

    /// <summary>
    /// Event raised when help was requested, either because the <see cref="Button.Help"/> button was
    /// clicked, or the F1 key was pressed.
    /// </summary>
    public event EventHandler? HelpRequested;

    /// <summary>Event raised when a hyperlink defined in the text areas of the dialog was clicked.</summary>
    public event EventHandler<HyperlinkClickedEventArgs>? HyperlinkClicked;

    internal event EventHandler<Action<PageUpdateInfo>>? UpdateRequested;

    /// <summary>
    /// Exits the page. This may cause the dialog to close or a new page may be navigated to.
    /// </summary>
    /// <remarks>
    /// When this method is called, the return value of show methods and <see
    /// cref="NavigationRequest.ClickedButton"/> will be <see langword="null"/>.
    /// </remarks>
    public void Exit() => OnUpdateRequested(info => info.Dialog.SendMessage(TDM_CLICK_BUTTON, 0));

    /// <inheritdoc/>
    public void Dispose()
    {
        Buttons.Dispose();
        Expander?.Dispose();
        RadioButtons.Dispose();
        _content.Dispose();
        _footerText.Dispose();
        _windowTitle.Dispose();
        _mainInstruction.Dispose();
        GC.SuppressFinalize(this);
    }

    internal HRESULT HandleNotification(Notification notif)
    {
        switch (notif.Id)
        {
            // Recieved after TDN_DIALOG_CONSTRUCTED
            case TDN_CREATED:
                Created.Raise(this);
                break;

            // Recieved by the new page, after TDN_DIALOG_CONSTRUCTED
            case TDN_NAVIGATED:
                Created.Raise(this);
                break;

            // Also sent when the dialog window was closed.
            case TDN_BUTTON_CLICKED:
                return HandleClickNotification(notif);

            case TDN_HYPERLINK_CLICKED:
                HyperlinkClicked?.Invoke(this, new(Marshal.PtrToStringUni(notif.LParam)));
                break;

            // Sent before TDN_CREATED. TDN_CREATED is not sent after navigation.
            case TDN_DIALOG_CONSTRUCTED:
                // Needed for having an icon after the header was set.
                OnUpdateRequested(Icon.GetUpdate(TDIE_ICON_MAIN));
                break;

            // Sent after TDN_BUTTON_CLICKED on Help button click, unless S_FALSE was returned.
            case TDN_HELP:
                HelpRequested.Raise(this);
                break;
        }
        return _parts.ForwardNotification(notif);
    }

    internal TASKDIALOGCONFIG SetupConfig(TaskDialogCallbackProc callback)
    {
        TASKDIALOGCONFIG config = new()
        {
            Content = Content,
            dwFlags = CombineFlags((AllowHyperlinks, TDF_ENABLE_HYPERLINKS),
                                   (IsCancelable, TDF_ALLOW_DIALOG_CANCELLATION),
                                   (IsMinimizable, TDF_CAN_BE_MINIMIZED),
                                   (IsRightToLeftLayout, TDF_RTL_LAYOUT)),
            Footer = FooterText,
            MainInstruction = MainInstruction,
            pfCallbackProc = callback,
            WindowTitle = WindowTitle
        };
        _icon.SetIn(config, TDIE_ICON_MAIN);
        _footerIcon.SetIn(config, TDIE_ICON_FOOTER);
        _parts.SetPartsIn(config);
        return config;

        static TASKDIALOG_FLAGS CombineFlags(params (bool isSet, TASKDIALOG_FLAGS value)[] flags)
            => flags.Aggregate(default(TASKDIALOG_FLAGS), (intermediate, flag) => intermediate.SetFlags(flag.value, flag.isSet));
    }

    internal TASKDIALOGCONFIG SetupConfig(TaskDialogCallbackProc callback, HWND parent, WindowLocation startupLocation)
    {
        var config = SetupConfig(callback);
        config.hwndParent = parent;
        config.dwFlags.SetFlag(TDF_POSITION_RELATIVE_TO_WINDOW, startupLocation is WindowLocation.CenterParent);
        return config;
    }

    internal ButtonBase? GetClickedButton(int pnButton) => pnButton == 0 ? null : Buttons.GetItem(pnButton) ?? CommonButton.FromId(pnButton);

    private HRESULT HandleClickNotification(Notification notif)
    {
        // Raise the Click event for the appropriate button.
        if (Buttons.HandleNotification(notif) == HRESULT.S_FALSE)
        {
            // Click event was canceled, do not raise exit and prevent the dialog from closing.
            return HRESULT.S_FALSE;
        }

        var clickedButton = GetClickedButton((int)notif.WParam);

        // Do not raise Exiting for a non-committing button.
        if (IsCommitting(clickedButton))
        {
            ExitEventArgs e = new(clickedButton);
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