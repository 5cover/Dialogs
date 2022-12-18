using System.ComponentModel;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog control that induces commitment.</summary>
public abstract class CommitControl : DialogControl<IdControlUpdateInfo>
{
    private bool _isEnabled = true;

    private bool _requiresElevation;

    /// <summary>Event raised when this commit control is clicked.</summary>
    /// <remarks>
    /// Set the <see cref="CancelEventArgs.Cancel"/> property of the event arguments to <see langword="true"/> to prevent the
    /// commit control from closing its containing page.
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
            RequestIsEnabledUpdate();
        }
    }

    /// <summary>Gets or sets whether an User Account Control (UAC) shield icon is displayed near the commit control.</summary>
    /// <remarks>Default value is <see langword="false"/>.</remarks>
    public bool RequiresElevation
    {
        get => _requiresElevation;
        set
        {
            _requiresElevation = value;
            RequestElevationUpdate();
        }
    }

    /// <summary>Simulates a click on this button.</summary>
    public void Click() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_BUTTON, update.ControlId));

    internal override HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_BUTTON_CLICKED)
        {
            CancelEventArgs e = new();
            Clicked?.Invoke(this, e);
            if (e.Cancel)
            {
                return HRESULT.S_FALSE;
            }
        }
        return base.HandleNotification(id, wParam, lParam);
    }

    private protected override void InitializeState()
    {
        RequestElevationUpdate();
        RequestIsEnabledUpdate();
    }

    private void RequestElevationUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE, update.ControlId, _requiresElevation));

    private void RequestIsEnabledUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_ENABLE_BUTTON, update.ControlId, _isEnabled));
}

/// <summary>A dialog commit control with text.</summary>
/// <remarks>This class implements <see cref="IDisposable"/>.</remarks>
public abstract class TextCommitControl : CommitControl, ITextControl, IDisposable
{
    private readonly SafeLPWSTR _nativeText = SafeLPWSTR.Null;

    private protected TextCommitControl(string? text) => _nativeText = new(text!); // !: null supported in base constructor.

    StrPtrUni ITextControl.NativeText => _nativeText;

    /// <inheritdoc/>
    public void Dispose()
    {
        _nativeText.Dispose();
        GC.SuppressFinalize(this);
    }
}