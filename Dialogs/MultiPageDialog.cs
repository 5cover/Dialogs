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
/// The navigation request. It contains the commit control that was clicked, as well as the type of the
/// request.
/// </param>
/// <returns>The next page to navigate to, or <see langword="null"/> to end the navigation.</returns>
public delegate Page? NextPageSelector(NavigationRequest navigationRequest);

/// <summary>A dialog with multiple pages and support for navigation.</summary>
/// <remarks>
/// Navigation occurs when <see cref="Navigate()"/> is called or when <see cref="Dialog.CurrentPage"/>
/// exits.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001", Justification = $"{nameof(_navigatedPagePtr)} is disposed at exit.")]
public class MultiPageDialog : Dialog
{
    private readonly IDictionary<Page, NextPageSelector> _nextPageSelectors;
    private SafeHGlobalHandle? _navigatedPagePtr;

    /// <param name="firstPage">The first page of the dialog.</param>
    /// <param name="nextPageSelectors">
    /// An dictionary of <see cref="NextPageSelector"/> delegates keyed by the page they navigate from.
    /// </param>
    public MultiPageDialog(Page firstPage, IDictionary<Page, NextPageSelector> nextPageSelectors) : base(firstPage)
    {
        _nextPageSelectors = nextPageSelectors;
        CurrentPage.Exiting += Navigate;
    }

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

    private static NavigationRequest MakeRequest(CommitControl? clickedControl)
        => new(clickedControl, clickedControl is null
                                   ? NavigationRequestKind.Exit
                                   : clickedControl.Equals(Button.Cancel)
                                       ? NavigationRequestKind.Cancel
                                       : NavigationRequestKind.Commit);

    private void Navigate(object? sender, ExitEventArgs e) => e.Cancel = Navigate(MakeRequest(e.ClickedControl));

    private bool Navigate(NavigationRequest navigationRequest)
    {
        _navigatedPagePtr?.Dispose();
        if (_nextPageSelectors.TryGetValue(CurrentPage, out var nextPageChooser)
          && nextPageChooser(navigationRequest) is { } nextPage)
        {
            CurrentPage.Exiting -= Navigate;
            CurrentPage.UpdateRequested -= PerformUpdate;
            CurrentPage.Showing = false;

            nextPage.Exiting += Navigate;
            nextPage.UpdateRequested += PerformUpdate;
            nextPage.Showing = true;

            CurrentPage = nextPage;
            _navigatedPagePtr = new(nextPage.SetupConfig(Callback).MarshalToPtr(Marshal.AllocHGlobal, out var bytesAllocated), bytesAllocated);
            _ = ((HWND)Handle).SendMessage(TDM_NAVIGATE_PAGE, 0, _navigatedPagePtr.DangerousGetHandle());
            return true;
        }
        return false;
    }
}