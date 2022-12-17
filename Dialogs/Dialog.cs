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

    /// <summary>Initializes a new instance of the <see cref="Dialog"/> class.</summary>
    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="chooseNextPage">
    /// For multi-page dialogs, a function that returns the next page to navigate to when the previous page has been closed, or
    /// <see langword="null"/> to end the dialog early. Specifiy <see langword="null"/> to have a single-page dialog.
    /// </param>
    public Dialog(Page firstPage, Func<CommitControl?, Page?>? chooseNextPage = null)
    {
        CurrentPage = firstPage;

        if (chooseNextPage is not null)
        {
            CurrentPage.Closing += NavigateNextPage;
        }

        void NavigateNextPage(object? sender, ControlClickedEventArgs args)
        {
            if (chooseNextPage(args.ClickedControl) is { } nextPage)
            {
                args.Cancel = true;
                _ = ((HWND)Handle).SendMessage(TaskDialogMessage.TDM_NAVIGATE_PAGE, lParam: CurrentPage.GetNative().MarshalToPtr(Marshal.AllocHGlobal, out _));
                CurrentPage = nextPage;
            }
        }
    }

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If <see cref="Show(nint?)"/> has not been called yet, the first page of the dialog, otherwise the page that is currently
    /// being displayed in the dialog.
    /// </value>
    public Page CurrentPage { get; private set; }

    /// <summary>Gets the window handle of the dialog window, or 0 if the dialog is currently not being shown.</summary>
    public nint Handle => CurrentPage.OwnerDialog.DangerousGetHandle();

    /// <summary>Gets or sets the window startup location.</summary>
    /// <value>The location of the dialog window when it is first shown. Default value is <see cref="WindowLocation.CenterScreen"/>.</value>
    public WindowLocation StartupLocation { get; set; }

    /// <summary>Gets or sets whether to use a <see cref="ComCtlV6ActivationContext"/> instance.</summary>
    /// <remarks>Default value is <see langword="true"/>.</remarks>
    internal bool UseActivationContext { get; set; } = true;

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

        using ComCtlV6ActivationContext activationContext = new(UseActivationContext);
        var config = CurrentPage.GetNative();

        config.hwndParent = owner ?? User32.GetActiveWindow();
        config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW, StartupLocation is WindowLocation.CenterParent);

        TaskDialogIndirect(config, out int pnButton, out int pnRadioButton, out _).ThrowIfFailed();
        return CurrentPage.GetResult(pnButton, pnRadioButton);
    }
}