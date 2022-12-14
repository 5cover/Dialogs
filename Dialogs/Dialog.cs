using System.Runtime.InteropServices;
using Scover.Dialogs.Parts;
using Vanara.Extensions;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>
/// A dialog box that can be used to display information and receive simple input from the user. Like a message box, it is
/// formatted by the operating system according to parameters you set. However, a dialog has many more features than a message box.
/// </summary>
public class Dialog
{
    /// <summary>The mnemonic (accelerator) prefix char used by all dialog controls.</summary>
    public const string MnemonicPrefix = "&";

    private Page _currentPage;

    /// <summary>Initializes a new instance of the <see cref="Dialog"/> class.</summary>
    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="chooseNextPage">
    /// For multi-page dialogs, a function that returns the next page to navigate to when the previous page has been closed, or
    /// <see langword="null"/> to end the dialog early. Specifiy <see langword="null"/> to have a single-page dialog.
    /// </param>
    public Dialog(Page firstPage, Func<CommitControl?, Page?>? chooseNextPage = null)
    {
        _currentPage = firstPage;

        if (chooseNextPage is not null)
        {
            CurrentPage.Closing += NavigateNextPage;
        }

        void NavigateNextPage(object? sender, ButtonClickedEventArgs args)
        {
            if (chooseNextPage(args.ClickedButton) is { } nextPage)
            {
                args.Cancel = true;
                CurrentPage = nextPage;
            }
        }
    }

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If <see cref="Show(nint?)"/> has not been called yet, the first page of the dialog, otherwise the page that is currently
    /// being displayed in the dialog.
    /// </value>
    public Page CurrentPage
    {
        get => _currentPage;
        private set
        {
            _currentPage?.Dispose();
            _currentPage = value;
            _ = ((HWND)Handle).SendMessage(TaskDialogMessage.TDM_NAVIGATE_PAGE, lParam: CurrentPage.GetNative().MarshalToPtr(Marshal.AllocHGlobal, out _));
        }
    }

    /// <summary>Gets the window handle of the dialog window, or 0 if the dialog is currently not being shown.</summary>
    public nint Handle => (nint)CurrentPage.OwnerDialog;

    /// <summary>Gets or sets the window startup location.</summary>
    /// <value>The location of the dialog window when it is first shown. Default value is <see cref="WindowLocation.CenterScreen"/>.</value>
    public WindowLocation StartupLocation { get; set; }

    /// <summary>Shows the dialog.</summary>
    /// <param name="owner">The owner window handle.</param>
    /// <returns>The result returned by the last page of the dialog.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    public DialogResult Show(nint? owner = null)
    {
        if (!WindowsVersion.SupportsTaskDialogs)
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.");
        }

        var config = CurrentPage.GetNative();
        using ComCtlV6ActivationContext activationContext = new(true);

        config.hwndParent = owner ?? User32.GetActiveWindow();
        config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW, StartupLocation is WindowLocation.CenterParent);

        TaskDialogIndirect(config, out int pnButton, out int pnRadioButton, out _).ThrowIfFailed();
        return CurrentPage.GetResult(pnButton, pnRadioButton);
    }
}