using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.User32;

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

    private readonly Queue<Action<PageUpdateInfo>> _queuedUpdates = new();
    private int _inCallback;

    static Dialog() => _ = new ComCtlV6ActivationContext(true);

    /// <param name="page">The page of the dialog.</param>
    public Dialog(Page page) => CurrentPage = page;

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If <see cref="Show(nint?)"/> has not been called yet, the first page of the dialog, otherwise the
    /// page that is currently being displayed in the dialog.
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

    /// <summary>Shows the dialog.</summary>
    /// <param name="owner">The owner window handle.</param>
    /// <returns>
    /// The <see cref="CommitControl"/> that was clicked, or <see langword="null"/> if <see
    /// cref="Page.Exit()"/> was called.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    /// <exception cref="EntryPointNotFoundException">
    /// One or more required <see langword="extern"/> functions could not be found.
    /// </exception>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// An error occuered while displaying the dialog.
    /// </exception>
    public CommitControl? Show(nint? owner = null)
    {
        CurrentPage.UpdateRequested += PerformUpdate;
        try
        {
            var config = CurrentPage.SetupConfig(Callback, owner ?? GetActiveWindow(), StartupLocation);
            CurrentPage.Showing = true;
            TaskDialogIndirect(config, out int pnButton, out _, out _).ThrowIfFailed();
            CurrentPage.Showing = false;
            return CurrentPage.GetClickedButton(pnButton);
        }
        catch (EntryPointNotFoundException e) when (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version < new Version(6, 0, 6000))
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.", e);
        }
        finally
        {
            CurrentPage.UpdateRequested -= PerformUpdate;
        }
    }

    /// <summary>Forcefully closes this dialog, ignoring navigation.</summary>
    /// <remarks>
    /// When this method is called, the return value of <see cref="Show(nint?)"/> will be <see
    /// cref="Button.Cancel"/>.
    /// </remarks>
    public virtual void Close() => CurrentPage.Exit();

    /// <summary>Performs an update using <see cref="Handle"/>.</summary>
    protected void PerformUpdate(object? sender, Action<PageUpdateInfo> update)
    {
        if (_inCallback > 0)
        {
            _queuedUpdates.Enqueue(update);
        }
        else
        {
            PerformUpdate(update);
        }
    }

    /// <summary>Performs an update using <see cref="Handle"/>.</summary>
    protected void PerformUpdate(Action<PageUpdateInfo> update) => update(new(Handle));

    /// <inheritdoc cref="TaskDialogCallbackProc"/>
    protected HRESULT Callback(HWND hwnd, TaskDialogNotification msg, nint wParam, nint lParam, nint refData)
    {
        ++_inCallback;
        try
        {
            Handle = hwnd.DangerousGetHandle();
            Notification notif = new(msg, wParam, lParam);
            return CurrentPage.HandleNotification(notif);
        }
        finally
        {
            if (--_inCallback == 0)
            {
                while (_queuedUpdates.Any())
                {
                    PerformUpdate(_queuedUpdates.Dequeue());
                }
            }
        }
    }
}