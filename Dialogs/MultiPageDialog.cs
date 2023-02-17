﻿using System.Runtime.InteropServices;

using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A dialog with multiple pages and support for navigation.</summary>
/// <remarks>
/// Navigation occurs when <see cref="Navigate()"/> is called or when <see cref="CurrentPage"/> is
/// committed. Navigation doesn't occur when the dialog window is closed.
/// </remarks>
public class MultiPageDialog : Dialog
{
    private readonly IDictionary<Page, NextPageSelector> _nextPageSelectors;

    private SafeHandle? _navigatedPagePtr;

    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="nextPageSelectors">
    /// An dictionary of <see cref="NextPageSelector"/> delegates keyed by the page they navigate from.
    /// </param>
    public MultiPageDialog(Page firstPage, IDictionary<Page, NextPageSelector> nextPageSelectors) : base(firstPage)
    {
        (CurrentPage, _nextPageSelectors) = (firstPage, nextPageSelectors);
        firstPage.Closing += Navigate;
    }

    /// <summary>Gets the current page.</summary>
    /// <value>
    /// If <see cref="Dialog.Show(nint?)"/> has not been called yet, the first page of the dialog, otherwise
    /// the page that is currently being displayed in the dialog.
    /// </value>
    public Page CurrentPage { get; private set; }

    /// <summary>Sends an explicit navigation request to the dialog.</summary>
    /// <returns>
    /// <see langword="true"/> if a navigation target page was found and successfully navigated to,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool Navigate() => Navigate(null);

    private void Navigate(object? sender, ClosingEventArgs e) => e.Cancel = e.ClickedControl is not null && Navigate(e.ClickedControl);

    private bool Navigate(CommitControl? clickedControl)
    {
        _navigatedPagePtr?.Dispose();
        if (_nextPageSelectors.TryGetValue(CurrentPage, out var nextPageChooser)
          && nextPageChooser(clickedControl) is { } nextPage)
        {
            CurrentPage.Closing -= Navigate;
            CurrentPage.UpdateRequested -= PerformUpdate;
            nextPage.Closing += Navigate;
            nextPage.UpdateRequested += PerformUpdate;

            _navigatedPagePtr = nextPage.CreateConfigPtr();
            _ = ((HWND)Handle).SendMessage(TaskDialogMessage.TDM_NAVIGATE_PAGE, 0, _navigatedPagePtr.DangerousGetHandle());

            CurrentPage = nextPage;
            return true;
        }
        return false;
    }
}