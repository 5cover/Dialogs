using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>
/// A dialog box that displays information and receives simple input from the user. Like a message box, it
/// is formatted by the operating system according to parameters you set. However, a dialog has many more
/// features than a message box.
/// </summary>
public class Dialog
{
    /// <summary>The mnemonic (accelerator) prefix used by all dialog controls.</summary>
    public const char MnemonicPrefix = '&';

    /// <param name="page">The page of the dialog.</param>
    public Dialog(Page page)
        => CurrentPage = page;

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If the dialog has not yet been shown, the first page of the dialog, otherwise the page that is
    /// currently being displayed in the dialog.
    /// </value>
    public Page CurrentPage { get; protected set; }

    /// <summary>
    /// Gets the window handle of the dialog, or 0 if the dialog is currently not being shown.
    /// </summary>
    public nint Handle { get; protected set; }

    /// <summary>Gets or sets the window startup location.</summary>
    /// <value>
    /// The location of the dialog window when it is first shown. Default value is <see
    /// cref="WindowLocation.CenterScreen"/>.
    /// </value>
    public WindowLocation StartupLocation { get; set; }

    /// <summary>Forcefully closes this dialog, ignoring navigation.</summary>
    /// <remarks>
    /// When this method is called, the returned button of show methods will be <see cref="Button.Cancel"/>.
    /// </remarks>
    public virtual void Close() => CurrentPage.Exit();

    /// <summary>Shows a modeless dialog.</summary>
    /// <returns>The <see cref="ButtonBase"/> that was clicked.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Could not show the dialog because Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    /// <exception cref="EntryPointNotFoundException">
    /// One or more required <see langword="extern"/> functions could not be found.
    /// </exception>
    /// <exception cref="InvalidOperationException">The dialog is already being shown.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// An error occured while displaying the dialog.
    /// </exception>
    public ButtonBase? Show() => Show(ParentWindow.Active);

    /// <summary>Shows a dialog.</summary>
    /// <param name="parent">The parent window to use.</param>
    /// <inheritdoc cref="Show()"/>
    public ButtonBase? Show(ParentWindow parent) => Show(parent.Hwnd);

    /// <summary>
    /// Shows a modal dialog with the active window attached to the calling thread's message queue as a
    /// parent.
    /// </summary>
    /// <param name="hwnd">
    /// The parent window handle. The dialog will be modal if the value differs from 0.
    /// </param>
    /// <inheritdoc cref="Show()"/>
    public ButtonBase? Show(nint hwnd)
    {
        if (CurrentPage.IsShown)
        {
            throw new InvalidOperationException("The dialog is already being shown.");
        }

        CurrentPage.UpdateRequested += PerformUpdate;
        var config = CurrentPage.SetupConfig(Callback, hwnd, StartupLocation);
        CurrentPage.IsShown = true;
        try
        {
            using (new ComCtlV6ActivationContext())
            {
                TaskDialogIndirect(config, out int pnButton, out _, out _).ThrowIfFailed();
                return CurrentPage.GetClickedButton(pnButton);
            }
        }
        catch (EntryPointNotFoundException e) when (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version < new Version(6, 0, 6000))
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.", e);
        }
        finally
        {
            CurrentPage.IsShown = false;
            CurrentPage.UpdateRequested -= PerformUpdate;
        }
    }

    /// <inheritdoc cref="TaskDialogCallbackProc"/>
    protected HRESULT Callback(HWND hwnd, TaskDialogNotification msg, nint wParam, nint lParam, nint refData)
    {
        Handle = hwnd.DangerousGetHandle();
        Notification notif = new(msg, wParam, lParam);
        return CurrentPage.HandleNotification(notif);
    }

    /// <summary>Performs an update using <see cref="Handle"/>.</summary>
    protected void PerformUpdate(object? sender, Action<PageUpdateInfo> update) => update(new(Handle));
}