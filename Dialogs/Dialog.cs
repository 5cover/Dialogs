using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

namespace Scover.Dialogs;

/// <summary>
/// A dialog box that displays information and receives simple input from the user. Like a message box, it is formatted by the
/// operating system according to parameters you set. However, a dialog has many more features than a message box.
/// </summary>
public class Dialog
{
    /// <summary>The mnemonic (accelerator) prefix used by all dialog controls.</summary>
    public const string MnemonicPrefix = "&";

    private readonly Page _firstPage;
    private HWND _handle;
    private SafeHandle? _navigatedPagePtr;

    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="chooseNextPage">
    /// For multi-page dialogs, a function that returns the next page to navigate to when the previous page has been closed, or
    /// <see langword="null"/> to end the dialog early. Specifiy <see langword="null"/> to have a single-page dialog.
    /// </param>
    public Dialog(Page firstPage, Func<Page, CommitControl, Page?>? chooseNextPage = null)
    {
        firstPage.HandleRecieved += (s, handle) => _handle = handle;
        firstPage.UpdateRequested += Update;
        CurrentPage = _firstPage = firstPage;

        if (chooseNextPage is not null)
        {
            firstPage.Closing += NavigateNextPage;
        }

        void NavigateNextPage(object? sender, CommitControlClickedEventArgs args)
        {
            if (chooseNextPage(CurrentPage, args.ClickedControl) is { } nextPage)
            {
                args.Cancel = true;

                nextPage.UpdateRequested += Update;
                nextPage.Closing += NavigateNextPage;

                _navigatedPagePtr?.Dispose();
                _navigatedPagePtr = nextPage.CreateConfigPtr();

                _ = _handle.SendMessage(TaskDialogMessage.TDM_NAVIGATE_PAGE, 0, _navigatedPagePtr.DangerousGetHandle());
                CurrentPage = nextPage;
            }
        }
        void Update(object? sender, Action<PageUpdateInfo> update) => update(new(_handle));
    }

    /// <summary>Gets or sets whether to use an activation context for calling <c>TaskDialogIndirect</c>.</summary>
    /// <remarks>
    /// If this is <see langword="false"/> and the calling project doesn't have a reference to ComCtl32 V6 in its manifest, <see
    /// cref="Show(nint?)"/> will throw <see cref="EntryPointNotFoundException"/>. Default value is <see langword="true"/>.
    /// </remarks>
    public static bool UseActivationContext { get; set; } = true;

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If <see cref="Show(nint?)"/> has not been called yet, the first page of the dialog, otherwise the page that is currently
    /// being displayed in the dialog.
    /// </value>
    public Page CurrentPage { get; private set; }

    /// <summary>Gets the window handle of the dialog, or 0 if the dialog is currently not being shown.</summary>
    public nint Handle => _handle.DangerousGetHandle();

    /// <summary>Gets or sets the window startup location.</summary>
    /// <value>The location of the dialog window when it is first shown. Default value is <see cref="WindowLocation.CenterScreen"/>.</value>
    public WindowLocation StartupLocation { get; set; }

    /// <summary>Shows the dialog.</summary>
    /// <param name="owner">The owner window handle.</param>
    /// <returns>
    /// The <see cref="CommitControl"/> that was clicked or <see cref="Button.Cancel"/> if the dialog was closed using Alt-F4,
    /// Escape, or the title bar's close button.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Cannot show the dialog becuase Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    public CommitControl Show(nint? owner = null)
    {
        try
        {
            using ComCtlV6ActivationContext? activationContext = new(UseActivationContext);
            return _firstPage.Show(owner ?? GetActiveWindow(), StartupLocation);
        }
        catch (EntryPointNotFoundException e)
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.", e);
        }
    }
}