using System.Diagnostics;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A dialog expander control.</summary>
/// <remarks>This class cannot be inherited and implements <see cref="IDisposable"/>.</remarks>
[DebuggerDisplay($"{{{nameof(Text)}}}")]
public sealed class Expander : DialogControl<PageUpdateInfo>, IDisposable
{
    private SafeLPWSTR _nativeText;

    /// <param name="text">The expanded information of the footer.</param>
    // An non-empty initial value is needed for the expander to be visible when no text is set. Set it to the Zero Width Space character.
    public Expander(string? text = null) => _nativeText = new(text ?? "\u200B");

    /// <summary>Event raised when the expander is expanded or collapsed.</summary>
    public event EventHandler? ExpandedChanged;

    /// <summary>Gets the expander collapse button text.</summary>
    /// <remarks>If the value is <see langword="null"/>, the text "Collapse" will be shown near the collapse button.</remarks>
    /// <value>The text to show near the collapse button of the expander. Default value is <see langword="null"/>.</value>
    public string? CollapseButtonText { get; init; }

    /// <summary>Gets the expander expand button text.</summary>
    /// <remarks>If the value is <see langword="null"/>, the text "Expand" will be shown near the expand button.</remarks>
    /// <value>The text to show near the expand button of the expander. Default value is <see langword="null"/>.</value>
    public string? ExpandButtonText { get; init; }

    /// <summary>Gets the position.</summary>
    /// <value>The position of the <see cref="Text"/> when the expander is expanded. Default value is <see cref="ExpanderPosition.BelowContent"/>.</value>
    public ExpanderPosition ExpanderPosition { get; init; } = ExpanderPosition.BelowContent;

    /// <summary>Gets or sets whether the expander is expanded.</summary>
    /// <remarks>Setting this property while the dialog is shown will have no effect.</remarks>
    /// <value>
    /// <see langword="true"/> when the expander when the page is initially displayed, <see langword="false"/> otherwise.
    /// Default value is <see langword="false"/>.
    /// </value>
    public bool IsExpanded { get; set; }

    /// <summary>Gets or sets the text.</summary>
    /// <remarks>If the value is <see cref="string.Empty"/>, the expanded area will be empty.</remarks>
    /// <value>The expanded information. Default value is <see cref="string.Empty"/>.</value>
    public string Text
    {
        get => (string?)_nativeText ?? "";
        set
        {
            _nativeText.Dispose();
            _nativeText = new(value);
            RequestUpdate(UpdateExpandedInformation);
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _nativeText.Dispose();

    /// <remarks>
    /// <inheritdoc path="/remarks"/>
    /// <item>
    /// <term><see cref="TaskDialogNotification.TDN_EXPANDO_BUTTON_CLICKED"/></term>
    /// <term>Raises <see cref="ExpandedChanged"/></term>
    /// <term><see langword="null"/></term>
    /// </item>
    /// </remarks>
    /// <inheritdoc/>
    internal override HRESULT? HandleNotification(Notification notif)
    {
        if (notif.Id is TaskDialogNotification.TDN_EXPANDO_BUTTON_CLICKED)
        {
            IsExpanded = Convert.ToBoolean(notif.WParam);
            ExpandedChanged.Raise(this);
        }
        return base.HandleNotification(notif);
    }

    internal override void SetIn(in TASKDIALOGCONFIG container)
    {
        (container.CollapsedControlText, container.ExpandedControlText, container.pszExpandedInformation) = (ExpandButtonText, CollapseButtonText, _nativeText);
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_EXPANDED_BY_DEFAULT, IsExpanded);
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_EXPAND_FOOTER_AREA, ExpanderPosition is ExpanderPosition.BelowFooter);
    }

    private protected override void InitializeState() => RequestUpdate(UpdateExpandedInformation);

    private void UpdateExpandedInformation(PageUpdateInfo info)
        => info.Dialog.SendMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, TASKDIALOG_ELEMENTS.TDE_EXPANDED_INFORMATION, _nativeText.DangerousGetHandle());
}