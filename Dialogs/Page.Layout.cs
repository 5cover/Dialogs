using Scover.Dialogs.Parts;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

// Layout properties can only be set during initialization using the object initializer syntax. However, they still follow the
// state of the dialog at showtime. Another page must be navigated to in the containing dialog in order to change these
// properties after initialization. Disposable layout properties are disposed, because they are considered to be owned by the page.

// Layout is { get; init; }

public partial class Page
{
    /// <summary>Gets whether to allow hyperlinks</summary>
    /// <remarks>
    /// <para>
    /// When this property is <see langword="true"/>, the <see cref="Content"/>, <see cref="Expander.Text"/> and
    /// <see cref="FooterText"/> properties can use hyperlinks in the following form: <c>&lt;A
    /// HREF="executablestring"&gt;Hyperlink NativeText&lt;/A&gt;</c>
    /// </para>
    /// Enabling hyperlinks when using content from an unsafe source may cause security vulnerabilities.
    /// <para>
    /// Dialogs will not actually execute hyperlinks. To take action when the user presses a hyperlink, handle the <see
    /// cref="HyperlinkClicked"/> event.
    /// </para>
    /// </remarks>
    /// <value>
    /// <see langword="true"/> when hyperlinks are allowed for the <see cref="Content"/>, <see
    /// cref="Expander.Text"/> and <see cref="FooterText"/> properties; otherwise, <see langword="false"/>.
    /// Default value is <see langword="false"/>.
    /// </value>
    public bool AllowHyperlinks
    {
        get => _config.dwFlags.HasFlag(TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS);
        init => _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS, value);
    }

    /// <summary>Gets the commit control collection.</summary>
    /// <remarks>
    /// If the value is <see langword="null"/>, the <see cref="Button.OK"/> button will be shown by default. The referenced
    /// object is disposed with the page.
    /// </remarks>
    /// <value>The collection of all the commit controls, buttons or command links. Default value is <see langword="null"/>.</value>
    public CommitControlCollection? Buttons
    {
        get => _parts.Get<CommitControlCollection>();
        init => _parts.Set(value);
    }

    /// <summary>Gets the expander.</summary>
    /// <value>
    /// The expander area or <see langword="null"/> if it is not defined. Default value is <see langword="null"/>. The
    /// referenced object is disposed with the page.
    /// </value>
    public Expander? Expander
    {
        get => _parts.Get<Expander>();
        init => _parts.Set(value);
    }

    /// <summary>Gets the header.</summary>
    /// <remarks>If the value is <see cref="DialogHeader.None"/>, no header bar will be displayed.</remarks>
    /// <value>The header bar. Default value is <see cref="DialogHeader.None"/>.</value>
    public DialogHeader Header
    {
        get => _parts.Get<DialogHeader>().AssertNotNull();
        init => _parts.Set(value);
    }

    /// <summary>Gets whether to allow cancelation.</summary>
    /// <value>
    /// <see langword="true"/> if the owner dialog window can be closed using Alt-F4, Escape or the title bar's close button
    /// even if no cancel button is specified, <see langword="false"/> otherwise. Default value is <see langword="false"/>.
    /// </value>
    public bool IsCancelable
    {
        get => _config.dwFlags.HasFlag(TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION);
        init => _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION, value);
    }

    /// <summary>Gets whether to allow minimization.</summary>
    /// <value>
    /// Whether the owner dialog window has a minimize box on its title bar when it is shown modeless. Default value is <see langword="false"/>.
    /// </value>
    public bool IsMinimizable
    {
        get => _config.dwFlags.HasFlag(TASKDIALOG_FLAGS.TDF_CAN_BE_MINIMIZED);
        init => _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_CAN_BE_MINIMIZED, value);
    }

    /// <summary>Gets whether text is displayed right to left.</summary>
    /// <value>
    /// <see langword="true"/> when the content of the dialog is displayed right to left; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool IsRightToLeftLayout
    {
        get => _config.dwFlags.HasFlag(TASKDIALOG_FLAGS.TDF_RTL_LAYOUT);
        init => _config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_RTL_LAYOUT, value);
    }

    /// <summary>Gets the progress bar.</summary>
    /// <remarks>If the value is <see langword="null"/>, no progress bar will be shown.</remarks>
    /// <value>The progress bar of the page. Default value is <see langword="null"/>.</value>
    public ProgressBar? ProgressBar
    {
        get => _parts.Get<ProgressBar>();
        init => _parts.Set(value);
    }

    /// <summary>Gets the radio button list</summary>
    /// <remarks>
    /// If the value is <see langword="null"/>, no radio button will be shown. The referenced object is disposed with the page.
    /// </remarks>
    /// <value>The list of all the radio buttons. Default value is <see langword="null"/>.</value>
    public RadioButtonCollection? RadioButtons
    {
        get => _parts.Get<RadioButtonCollection>();
        init => _parts.Set(value);
    }

    /// <summary>Gets the sizing strategy.</summary>
    /// <value>The sizing strategy to size the owner dialog window. Default value is <see cref="Sizing.Automatic"/>.</value>
    public Sizing Sizing
    {
        get => _parts.Get<Sizing>().AssertNotNull();
        init => _parts.Set(value);
    }

    /// <summary>Gets the verification.</summary>
    /// <remarks>If the value is <see langword="null"/>, no verification will be shown.</remarks>
    /// <value>The verification of the page. Default value is <see langword="null"/>.</value>
    public Verification? Verification
    {
        get => _parts.Get<Verification>();
        init => _parts.Set(value);
    }

    /// <summary>Gets the window title.</summary>
    /// <remarks>If the value is <see langword="null"/>, the window title will be the filename of the current executable.</remarks>
    /// <value>The title of the owner dialog window. Default value is <see langword="null"/>.</value>
    public string? WindowTitle { get => _config.WindowTitle; init => _config.WindowTitle = value; }
}