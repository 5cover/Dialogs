using System.Runtime.InteropServices;
using Scover.Dialogs.Parts;
using Vanara.Extensions;
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

        void NavigateNextPage(object? sender, CommitControlClickedEventArgs args)
        {
            if (chooseNextPage(args.ClickedControl) is { } nextPage)
            {
                args.Cancel = true;
                _ = ((HWND)Handle).SendMessage(TaskDialogMessage.TDM_NAVIGATE_PAGE, 0, CurrentPage.Config.MarshalToPtr(Marshal.AllocHGlobal, out _));
                CurrentPage = nextPage;
            }
        }
    }

    /// <summary>Gets or sets whether to use a <see cref="ComCtlV6ActivationContext"/> instance.</summary>
    /// <remarks>Default value is <see langword="true"/>.</remarks>
    public static bool UseActivationContext { get; set; } = true;

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

    /// <summary>Shows the dialog.</summary>
    /// <param name="owner">The owner window handle.</param>
    /// <returns>
    /// The <see cref="CommitControl"/> that was clicked or <see langword="null"/> if <see cref="Button.Cancel"/> was clicked or
    /// the dialog was closed using Alt-F4, Escape, or the title bar's close button.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Cannot show the dialog becuase Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    public CommitControl? Show(nint? owner = null)
    {
        if (!WindowsVersion.SupportsTaskDialogs)
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.");
        }

        using ComCtlV6ActivationContext? activationContext = new(UseActivationContext);

        CurrentPage.Config.hwndParent = owner ?? GetActiveWindow();
        CurrentPage.Config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW, StartupLocation is WindowLocation.CenterParent);

        TaskDialogIndirect(CurrentPage.Config, out int pnButton, out _, out _).ThrowIfFailed();
        if (pnButton == Button.Cancel.Id)
        {
            return null;
        }
        if (CurrentPage.Buttons?.Any() ?? false)
        {
            return CurrentPage.Buttons.GetControlFromId(pnButton);
        }
        return CommonButton.FromId(pnButton);
    }
}