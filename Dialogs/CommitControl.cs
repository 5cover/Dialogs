using System.ComponentModel;

using Vanara.PInvoke;

namespace Scover.Dialogs;

/// <summary>A dialog control that induces commitment.</summary>
public abstract class CommitControl : DialogControl<IdControlUpdateInfo>
{
    private bool _isEnabled = true;

    private bool _requiresElevation;

    /// <summary>Event raised when this commit control is clicked.</summary>
    /// <remarks>
    /// Set the <see cref="CancelEventArgs.Cancel"/> property of the event arguments to <see
    /// langword="true"/> to prevent the commit control from closing its containing page.
    /// </remarks>
    public event EventHandler<CancelEventArgs>? Clicked;

    /// <summary>Gets or sets whether this commit control is enabled.</summary>
    /// <remarks>Default value is <see langword="true"/>.</remarks>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            RequestUpdate(UpdateIsEnabled);
        }
    }

    /// <summary>
    /// Gets or sets whether an User Account Control (UAC) shield icon is displayed near the commit control.
    /// </summary>
    /// <remarks>Default value is <see langword="false"/>.</remarks>
    public bool RequiresElevation
    {
        get => _requiresElevation;
        set
        {
            _requiresElevation = value;
            RequestUpdate(UpdateElevation);
        }
    }

    /// <summary>Simulates a click on this button.</summary>
    public void Click() => RequestUpdate(info => info.Dialog.SendMessage(TDM_CLICK_BUTTON, info.ControlId));

    /// <remarks>
    /// <list type="table">
    /// <inheritdoc path="//remarks//listheader"/><inheritdoc path="//remarks//item"/>
    /// <item>
    /// <term><see cref="TDN_BUTTON_CLICKED"/></term>
    /// <term>Raises <see cref="Clicked"/></term>
    /// <term>
    /// <see cref="HRESULT.S_FALSE"/> if <see cref="CancelEventArgs.Cancel"/> was <see langword="true"/>
    /// </term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <inheritdoc/>
    internal override HRESULT HandleNotification(Notification notif)
    {
        _ = base.HandleNotification(notif);
        if (notif.Id is TDN_BUTTON_CLICKED)
        {
            CancelEventArgs e = new();
            Clicked?.Invoke(this, e);
            if (e.Cancel)
            {
                return HRESULT.S_FALSE;
            }
        }
        return default;
    }

    /// <inheritdoc/>
    protected override void InitializeState() => RequestUpdate(info =>
    {
        UpdateElevation(info);
        UpdateIsEnabled(info);
    });

    private void UpdateElevation(IdControlUpdateInfo info) => info.Dialog.SendMessage(TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE, info.ControlId, _requiresElevation);

    private void UpdateIsEnabled(IdControlUpdateInfo info) => info.Dialog.SendMessage(TDM_ENABLE_BUTTON, info.ControlId, _isEnabled);
}