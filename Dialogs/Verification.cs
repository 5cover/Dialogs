﻿using System.Diagnostics;

using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A dialog verification checkbox control.</summary>
/// <remarks>This class cannot be inherited.</remarks>
[DebuggerDisplay($"{{{nameof(Text)}}}")]
public sealed class Verification : DialogControl<PageUpdateInfo>
{
    private bool _isChecked;

    /// <param name="text">The text to show near the verification checkbox.</param>
    public Verification(string text) => Text = text;

    /// <summary>Event raise when the verification is checked.</summary>
    public event EventHandler? Checked;

    /// <summary>Gets or sets whether the verification is checked.</summary>
    /// <value>
    /// <see langword="true"/> if the verficiation checkbox is checked, <see langword="false"/> otherwise.
    /// </value>
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            RequestUpdate(UpdateIsChecked);
        }
    }

    /// <summary>Gets the verification text.</summary>
    /// <remarks>
    /// If the value is <see cref="string.Empty"/>, no verification checkbox will be shown.
    /// </remarks>
    /// <value>
    /// The text shown next to the verification checkbox. Default value is <see cref="string.Empty"/>.
    /// </value>
    public string Text { get; } = "";

    /// <summary>Sets the keyboard focus to the verification checkbox of the dialog, if it exists.</summary>
    public void Focus() => RequestUpdate(info => info.Dialog.SendMessage(TDM_CLICK_VERIFICATION, IsChecked, true));

    /// <remarks>
    /// <list type="table">
    /// <inheritdoc path="//remarks//listheader"/><inheritdoc path="//remarks//item"/>
    /// <item>
    /// <term><see cref="TDN_VERIFICATION_CLICKED"/></term>
    /// <term>Raises <see cref="Checked"/></term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <inheritdoc/>
    internal override HRESULT HandleNotification(Notification notif)
    {
        _ = base.HandleNotification(notif);
        if (notif.Id is TDN_VERIFICATION_CLICKED)
        {
            _isChecked = Convert.ToBoolean(notif.WParam);
            Checked.Raise(this);
        }
        return default;
    }

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        config.dwFlags.SetFlag(TDF_VERIFICATION_FLAG_CHECKED, IsChecked);
        config.VerificationText = Text;
    }

    private void UpdateIsChecked(PageUpdateInfo info) => info.Dialog.SendMessage(TDM_CLICK_VERIFICATION, _isChecked, false);
}