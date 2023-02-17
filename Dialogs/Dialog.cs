using Vanara.PInvoke;

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
    public const string MnemonicPrefix = "&";

    private readonly Page _firstPage;

    static Dialog() => _ = new ComCtlV6ActivationContext(true);

    /// <param name="page">The page of the dialog.</param>
    public Dialog(Page page) => _firstPage = page;

    /// <summary>
    /// Gets the window handle of the dialog, or 0 if the dialog is currently not being shown.
    /// </summary>
    public nint Handle { get; private protected set; }

    /// <summary>Gets or sets the window startup location.</summary>
    /// <value>
    /// The location of the dialog window when it is first shown. Default value is <see
    /// cref="WindowLocation.CenterScreen"/>.
    /// </value>
    public WindowLocation StartupLocation { get; set; }

    /// <summary>Shows the dialog.</summary>
    /// <param name="owner">The owner window handle.</param>
    /// <returns>
    /// The <see cref="CommitControl"/> that was clicked or <see langword="null"/> if the dialog was closed
    /// using Alt-F4, Escape, or the title bar's close button.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.
    /// </exception>
    /// <exception cref="EntryPointNotFoundException">
    /// One or more required <see langword="extern"/> functions could not be found.
    /// </exception>
    public CommitControl? Show(nint? owner = null)
    {
        _firstPage.HandleRecieved += SetHandle;
        _firstPage.UpdateRequested += PerformUpdate;
        try
        {
            return _firstPage.Show(owner ?? GetActiveWindow(), StartupLocation);
        }
        catch (EntryPointNotFoundException e) when (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version < new Version(6, 0, 6000))
        {
            throw new PlatformNotSupportedException("Can't show the dialog becuase Windows Task Dialogs require Windows Vista or later.", e);
        }
        finally
        {
            _firstPage.HandleRecieved -= SetHandle;
            _firstPage.UpdateRequested -= PerformUpdate;
        }

        void SetHandle(object? sender, HWND handle) => Handle = handle.DangerousGetHandle();
    }

    /// <summary>Performs an update using <see cref="Handle"/>.</summary>
    protected void PerformUpdate(object? sender, Action<PageUpdateInfo> update) => update(new(Handle));
}