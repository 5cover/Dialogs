﻿using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog expander control. This class cannot be inherited. This class implements <see cref="IDisposable"/>.</summary>
public sealed class Expander : ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, INotificationHandler, IStateInitializer, IDisposable
{
    // A default value is needed for the expander to be initialized in the config.
    private SafeLPWSTR _expandedInformation = new(" ");

    /// <summary>Event raised when the expander is expanded or collapsed.</summary>
    public event EventHandler? ExpandedChanged;

    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }

    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets the expander collapse button text.</summary>
    /// <remarks>If the value is <see langword="null"/>, the text "Collapse" will be shown near the collapse button.</remarks>
    /// <value>The text to show near the collapse button of the expander. Default value is <see langword="null"/>.</value>
    public string? CollapseButtonText { get; init; }

    /// <summary>Gets the expander expand button text.</summary>
    /// <remarks>If the value is <see langword="null"/>, the text "Expand" will be shown near the expand button.</remarks>
    /// <value>The text to show near the expand button of the expander. Default value is <see langword="null"/>.</value>
    public string? ExpandButtonText { get; init; }

    /// <summary>Gets or sets the expanded information.</summary>
    /// <remarks>If the value is <see langword="null"/>, no text will be shown in the expanded area.</remarks>
    /// <value>The text shown in the expanded area. Default value is <see langword="null"/>.</value>
    public string? ExpandedInformation
    {
        get => _expandedInformation;
        set
        {
            _expandedInformation.Dispose();
            _expandedInformation = new(value!);
            RequestExpandedInformationUpdate();
        }
    }

    /// <summary>Gets the position.</summary>
    /// <value>The position of the <see cref="ExpandedInformation"/> when the expander is expanded. Default value is <see cref="ExpanderPosition.BelowContent"/>.</value>
    public ExpanderPosition ExpanderPosition { get; init; } = ExpanderPosition.BelowContent;

    /// <summary>Gets whether to the expander is expanded.</summary>
    /// <remarks>Setting this property while the dialog is shown will have no effect.</remarks>
    /// <value>
    /// <see langword="true"/> when the expander when the page is initially displayed, <see langword="false"/> otherwise.
    /// Default value is <see langword="false"/>.
    /// </value>
    public bool IsExpanded { get; set; }

    /// <inheritdoc/>
    public void Dispose() => _expandedInformation.Dispose();

    HRESULT INotificationHandler.HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_EXPANDO_BUTTON_CLICKED)
        {
            IsExpanded = Convert.ToBoolean(wParam);
            ExpandedChanged.Raise(this);
        }
        return default;
    }

    void IStateInitializer.InitializeState() => RequestExpandedInformationUpdate();

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container)
    {
        (container.CollapsedControlText, container.ExpandedControlText) = (ExpandButtonText, CollapseButtonText);
        container.pszExpandedInformation = _expandedInformation;
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_EXPANDED_BY_DEFAULT, IsExpanded);
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_EXPAND_FOOTER_AREA, ExpanderPosition is ExpanderPosition.BelowFooter);
    }

    private void OnUpdateRequested(Action<PageUpdate> update) => UpdateRequested?.Invoke(this, update);

    private void RequestExpandedInformationUpdate()
        => OnUpdateRequested(update => _ = update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, TASKDIALOG_ELEMENTS.TDE_EXPANDED_INFORMATION, _expandedInformation.DangerousGetHandle()));
}