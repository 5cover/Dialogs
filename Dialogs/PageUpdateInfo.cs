﻿using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>Page update information.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public readonly struct PageUpdateInfo
{
    internal PageUpdateInfo(HWND dialog) => Dialog = dialog;

    internal HWND Dialog { get; }
}

internal record struct Notification(TaskDialogNotification Id, nint WParam, nint LParam);