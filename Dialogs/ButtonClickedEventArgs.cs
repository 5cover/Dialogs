﻿using System.ComponentModel;
using Scover.Dialogs.Parts;

namespace Scover.Dialogs;

/// <summary>Provides data for the an event about a click on a commit control.</summary>
public sealed class ButtonClickedEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonClickedEventArgs"/> class with the <see
    /// cref="CancelEventArgs.Cancel"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="clickedCotnrol">The commit control that initiated the closing of the dialog.</param>
    public ButtonClickedEventArgs(CommitControl? clickedCotnrol) => ClickedControl = clickedCotnrol;

    /// <summary>Gets the commit control that initiated the closing of the dialog.</summary>
    public CommitControl? ClickedControl { get; set; }
}