using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A dialog verification checkbox control. This class cannot be inherited.</summary>
public sealed class Verification : ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, INotificationHandler
{
    private bool _isChecked;

    /// <summary>Initializes a new instance of the <see cref="Verification"/> class.</summary>
    /// <param name="text">The text to show near the verification checkbox.</param>
    public Verification(string text) => Text = text;

    /// <summary>Event raise when the verification is checked.</summary>
    public event EventHandler? Checked;

    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }

    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets or sets whether the verification is checked.</summary>
    /// <value><see langword="true"/> if the verficiation checkbox is checked, <see langword="false"/> otherwise.</value>
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            RequestIsCheckedUpdate();
        }
    }

    /// <summary>Gets the verification text.</summary>
    /// <remarks>If the value is <see langword="null"/>, no verification checkbox will be shown.</remarks>
    /// <value>The text shown next to the verification checkbox. Default value is <see langword="null"/>.</value>
    public string? Text { get; }

    /// <summary>Sets the keyboard focus to the verification checkbox of the dialog, if it exists.</summary>
    public void Focus() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_VERIFICATION, IsChecked, true));

    HRESULT INotificationHandler.HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        if (id is TaskDialogNotification.TDN_VERIFICATION_CLICKED)
        {
            _isChecked = Convert.ToBoolean(wParam);
            Checked.Raise(this);
        }
        return default;
    }

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container)
    {
        container.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED, IsChecked);
        container.VerificationText = Text;
    }

    private void OnUpdateRequested(Action<PageUpdate> args) => UpdateRequested?.Invoke(this, args);

    private void RequestIsCheckedUpdate() => OnUpdateRequested(update => update.Dialog.SendMessage(TaskDialogMessage.TDM_CLICK_VERIFICATION, _isChecked, false));
}