using System.Runtime.InteropServices;

using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace Scover.Dialogs;

/// <summary>
/// A delegate that selects the next page to navigate to after a page closing or an explicit navigation
/// request.
/// </summary>
/// <param name="navigationRequest">
/// The navigation request. It contains the button that was clicked, as well as the type of the request.
/// </param>
/// <returns>The next page to navigate to, or <see langword="null"/> to end the navigation.</returns>
public delegate Page? NextPageSelector(NavigationRequest navigationRequest);

/// <summary>The kind of a navigation request.</summary>
public enum NavigationRequestKind
{
    /// <summary>
    /// Navigation was explicitly requested by calling <see cref="MultiPageDialog.Navigate()"/>
    /// </summary>
    Explicit,

    /// <summary>
    /// Navigation was requested since the dialog window was closed or <see cref="Button.Cancel"/> button
    /// was clicked.
    /// </summary>
    Cancel,

    /// <summary>Navigation was requested since <see cref="Page.Exit()"/> was called.</summary>
    Exit,

    /// <summary>Navigation was requested since a button was clicked.</summary>
    Commit,
}

/// <summary>A dialog with multiple pages and support for navigation.</summary>
/// <remarks>
/// Navigation occurs when <see cref="Navigate()"/> is called or when <see cref="Dialog.CurrentPage"/>
/// exits.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001", Justification = $"{nameof(_navigatedPagePtr)} is disposed at exit.")]
public class MultiPageDialog : Dialog
{
    private readonly Page _firstPage;
    private readonly IDictionary<Page, NextPageSelector> _nextPageSelectors;
    private SafeHGlobalHandle? _navigatedPagePtr;

    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="nextPageSelectors">
    /// An dictionary of <see cref="NextPageSelector"/> delegates keyed by the page they navigate from.
    /// </param>
    public MultiPageDialog(Page firstPage, IDictionary<Page, NextPageSelector> nextPageSelectors) : base(firstPage)
    {
        _firstPage = firstPage;
        _nextPageSelectors = nextPageSelectors;
        CurrentPage.Exiting += Navigate;
    }

    /// <inheritdoc cref="MultiPageDialog(Page, IDictionary{Page, NextPageSelector})"/>
    // concrete dictionary argument required for compact new syntax
    public MultiPageDialog(Page firstPage, Dictionary<Page, NextPageSelector> nextPageSelectors)
        : this(firstPage, (IDictionary<Page, NextPageSelector>)nextPageSelectors) { }

    /// <inheritdoc/>
    public override void Close()
    {
        CurrentPage.Exiting -= Navigate;
        base.Close();
        _navigatedPagePtr?.Dispose();
    }

    /// <summary>Sends an explicit navigation request to the dialog.</summary>
    /// <returns>
    /// <see langword="true"/> if a navigation target page was found and successfully navigated to,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool Navigate() => Navigate(new(null, NavigationRequestKind.Explicit));

    private static NavigationRequest MakeRequest(ButtonBase? clickedButton)
        => new(clickedButton, clickedButton is null
                                   ? NavigationRequestKind.Exit
                                   : clickedButton.Equals(Button.Cancel)
                                       ? NavigationRequestKind.Cancel
                                       : NavigationRequestKind.Commit);

    private void Navigate(object? sender, ExitEventArgs e) => e.Cancel = Navigate(MakeRequest(e.ClickedButton));

    private bool Navigate(NavigationRequest navigationRequest)
    {
        _navigatedPagePtr?.Dispose();
        if (_nextPageSelectors.TryGetValue(CurrentPage, out var nextPageSelector) && nextPageSelector(navigationRequest) is { } nextPage)
        {
            _navigatedPagePtr = new(nextPage.SetupConfig(Callback).MarshalToPtr(Marshal.AllocHGlobal, out var bytesAllocated), bytesAllocated);

            CurrentPage.Exiting -= Navigate;
            CurrentPage.UpdateRequested -= PerformUpdate;
            nextPage.IsShown = true;

            nextPage.Exiting += Navigate;
            nextPage.UpdateRequested += PerformUpdate;
            CurrentPage.IsShown = false;

            // As CurrentPage is used to identify the Page on which to call HandleNotification in Callback,
            // it must be set to the correct value before sending TDN_NAVIGATE_PAGE.
            CurrentPage = nextPage;
            _ = ((HWND)Handle).SendMessage(TDM_NAVIGATE_PAGE, 0, _navigatedPagePtr.DangerousGetHandle());

            return true;
        }

        // Go back to first page in case the dialog is shown again.
        CurrentPage = _firstPage;
        return false;
    }
}