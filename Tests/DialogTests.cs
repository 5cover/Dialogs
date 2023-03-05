using NUnit.Framework;

using Scover.Dialogs;

using Vanara.PInvoke;

namespace Tests;

[TestFixture]
public sealed class DialogTests
{
    [Test]
    public void MegaTest()
    {
        Button button2 = new("Button #2");
        using Page page = new()
        {
            AllowHyperlinks = true,
            IsMinimizable = true,
            IsRightToLeftLayout = false,
            Sizing = Sizing.FromWidth(500, DistanceUnit.DLU),
            WindowTitle = nameof(MegaTest),
            Header = DialogHeader.Yellow,
            Icon = DialogIcon.ErrorShield,
            MainInstruction = nameof(page.MainInstruction),
            Content = $"{nameof(page.Content)} with <A>hyperlink</A>. The progress bar should be half-filled.",
            ProgressBar = new ProgressBar() { Maximum = 90, Minimum = 80, Value = 85 },
            RadioButtons =
            {
                "Radio #1",
                "Radio #2",
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false }
            },
            Expander = new Expander()
            {
                Text = nameof(Expander.Text),
                CollapseButtonText = nameof(Expander.CollapseButtonText),
                ExpandButtonText = nameof(Expander.ExpandButtonText),
                ExpanderPosition = ExpanderPosition.BelowFooter,
                IsExpanded = true
            },
            Verification = new(nameof(page.Verification)) { IsChecked = true },
            Buttons = new(defaultItem: button2)
            {
                "Button #1",
                button2,
                Button.Cancel,
                new Button("Button #3 (Admin)") { RequiresElevation = true },
                new Button("Button #4 (Disabled)") { IsEnabled = false },
                new Button("Button #5 (Admin && Disabled)") { RequiresElevation = true, IsEnabled = false }
            },
            FooterIcon = DialogIcon.Information,
            FooterText = nameof(page.FooterText),
        };
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestAsyncProgress()
    {
        using Page page = new()
        {
            IsMinimizable = true,
            WindowTitle = nameof(TestAsyncProgress),
            MainInstruction = "Asynchronous operation in progress",
            Content = "The progress bar should increase and this window should close automatically.",
            ProgressBar = new()
        };

        page.Created += async (_, _) =>
        {
            while (page.ProgressBar.Value++ < 100)
            {
                await Task.Delay(50);
            }
            await Task.Delay(1000);
            page.Exit();
        };

        Assert.That(new Dialog(page).Show(), Is.Null);
    }

    [Test]
    public void TestAsyncProgressNavigation()
    {
        Button startNow = new("Start now", "The dialog will navigate to the second page and the operation will start");
        using Page page1 = new()
        {
            MainInstruction = "Start the operation?",
            Buttons = new(ButtonStyle.CommandLinks)
            {
                startNow,
                { "Abort", "The test will be aborted and nothing will happen (same as Cancel)" },
                Button.Cancel,
            }
        };
        using Page page2 = new()
        {
            IsMinimizable = true,
            WindowTitle = nameof(TestAsyncProgress),
            MainInstruction = "Asynchronous operation in progress",
            Content = "The progress bar should increase and this window should close automatically.",
            ProgressBar = new(),
        };
        page2.Created += async (_, _) =>
        {
            while (page2.ProgressBar.Value++ < 100)
            {
                await Task.Delay(50);
            }
            await Task.Delay(1000);
            page2.Exit();
        };
        Assert.That(new MultiPageDialog(page1, new()
        {
            [page1] = request => startNow.Equals(request.ClickedButton) ? page2 : null,
            [page2] = request =>
            {
                Assert.That(request.Kind, Is.EqualTo(NavigationRequestKind.Exit));
                return null;
            }
        }).Show(), Is.Null);
    }

    [Test]
    public void TestCommandLinks()
    {
        Button commandLink2 = new("Command link #2 (default)", "This is the default command link.");
        using Page page = new()
        {
            WindowTitle = nameof(TestCommandLinks),
            Content = "Assert that the command links are displayed properly.",
            Buttons = new(ButtonStyle.CommandLinks, commandLink2)
            {
                "Command link #1",
                commandLink2,
                { "Command link #3", "Supplemental explanation" },
                new Button("Command link #4 (disabled)") { IsEnabled = false },
                new Button("Command link #5 (admin)") { RequiresElevation = true },
                new Button("Command link #6 (admin && disabled)") { IsEnabled = false, RequiresElevation = true },

                // Common buttons are also supported.
                Button.Abort,
                Button.Help,
            },
        };
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestCustomIcon()
    {
        ushort index = 0;
        using var hIcon = Win32Error.ThrowLastErrorIfInvalid(Shell32.ExtractAssociatedIcon(default, new(@"%SystemRoot%\System32\rstrui.exe"), ref index));
        DialogIcon customIcon = DialogIcon.FromHandle(hIcon.DangerousGetHandle());
        using Page page = new()
        {
            Content = "Assert that the icon (restore point) is displayed properly.",
            Icon = customIcon,
            WindowTitle = nameof(TestCustomIcon),
        };
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestIcon()
    {
        int cnt = 1;
        Button buttonNextIcon = new("Next icon");
        using Page page = new()
        {
            WindowTitle = nameof(TestIcon),
            Header = DialogHeader.Green,
            Icon = DialogIcon.Information,
            MainInstruction = "Main instruction",
            Content = GetContent("information"),
            Verification = new("Main instruction"),
            Buttons = { buttonNextIcon, Button.Close, },
        };
        page.Verification.IsChecked = true;
        page.Verification.Checked += (s, e) => page.MainInstruction = page.Verification.IsChecked ? "Main instruction" : "";
        buttonNextIcon.Clicked += (s, e) =>
        {
            e.Cancel = true;
            (page.Icon, string iconName) = (cnt++ % 4) switch
            {
                0 => (DialogIcon.None, "none"),
                1 => (DialogIcon.SuccessShield, "success shield"),
                2 => (DialogIcon.FromId(15), "executable"),
                3 => (DialogIcon.Warning, "warning")
            };
            page.Content = GetContent(iconName);
        };
        _ = new Dialog(page).Show();

        static string GetContent(string iconName) => $"Assert that the icon ({iconName}) is displayed properly.";
    }

    [Test]
    public void TestIsCancelable()
    {
        using Page page = new()
        {
            IsCancelable = true,
            WindowTitle = nameof(TestIsCancelable),
            Content = "Assert that this dialog can be closed.",
        };
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestIsMinizable()
    {
        using Page page = new()
        {
            IsMinimizable = true,
            WindowTitle = nameof(TestIsMinizable),
            Content = "Assert that this dialog can be minimized.",
        };
        Dialog dialog = new(page);
        _ = dialog.Show();
    }

    [Test]
    public void TestNavigation()
    {
        using Page page1 = new()
        {
            WindowTitle = nameof(TestNavigation),
            MainInstruction = "Page 1",
            Content = "First page with expander. Press F1 to navigate to Page 2.",
            Expander = new("Expanded information")
            {
                ExpandButtonText = "Custom expand",
                CollapseButtonText = "Custom collapse",
                IsExpanded = true,
            },
            Buttons = { Button.Yes, Button.No, Button.Cancel },
        };
        var radio2 = new RadioButton("Radio #2 (default && disabled)")
        {
            IsEnabled = false
        };
        using Page page2 = new()
        {
            WindowTitle = nameof(TestNavigation),
            MainInstruction = "Page 2",
            Content = "Second page with radio buttons. Press F1 to navigate to Page 3",
            RadioButtons = new(defaultItem: radio2)
            {
                "Radio #1",
                radio2
            },
            Buttons = { Button.Cancel },
        };
        using Page page3 = new()
        {
            WindowTitle = nameof(TestNavigation),
            MainInstruction = "Page 3",
            Content = "Third page with progress bar. It should be half-filled. Press F1 to go back to page 1.",
            ProgressBar = new()
            {
                Value = 5,
                Maximum = 10,
            },
            Buttons = { Button.Cancel },
        };

        MultiPageDialog dlg = new(page1, new Dictionary<Page, NextPageSelector>
        {
            [page1] = req => req.Kind is NavigationRequestKind.Commit or NavigationRequestKind.Explicit ? page2 : null,
            [page2] = req => req.Kind is NavigationRequestKind.Commit or NavigationRequestKind.Explicit ? page3 : null,
            [page3] = req => req.Kind is NavigationRequestKind.Commit or NavigationRequestKind.Explicit ? page1 : null,
        });

        page1.HelpRequested += Navigate;
        page2.HelpRequested += Navigate;
        page3.HelpRequested += Navigate;

        _ = dlg.Show();

        void Navigate(object? sender, EventArgs e) => dlg.Navigate();
    }

    [Test]
    public void TestProgressBar()
    {
        Button
            minPlus10 = new("Min + 10"),
            minMinus10 = new("Min - 10"),
            maxPlus10 = new("Max + 10"),
            maxMinus10 = new("Max - 10"),
            valuePlus10 = new("Value + 10"),
            valueMinus10 = new("Value - 10"),
            toggleMode = new("Change mode"),
            cycleState = new("Change state"),
            intervalPlus1 = new("Interval + 1"),
            intervalMinus1 = new("Interval - 1");
        using Page page = new()
        {
            IsCancelable = true,
            Sizing = Sizing.FromWidth(200),
            WindowTitle = nameof(TestProgressBar),
            Content = "Assert that the progress bar behaves properly.",
            ProgressBar = new(),
            Expander = new() { IsExpanded = true },
            Buttons =
            {
                minPlus10,
                minMinus10,
                maxPlus10,
                maxMinus10,
                valuePlus10,
                valueMinus10,
                toggleMode,
                cycleState,
                intervalPlus1,
                intervalMinus1,
            },
        };
        var pb = page.ProgressBar;
        UpdateExpandedInfo();
        minPlus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Minimum += 10;
            UpdateExpandedInfo();
        };
        minMinus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Minimum -= 10;
            UpdateExpandedInfo();
        };
        maxPlus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Maximum += 10;
            UpdateExpandedInfo();
        };
        maxMinus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Maximum -= 10;
            UpdateExpandedInfo();
        };
        valuePlus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Value += 10;
            UpdateExpandedInfo();
        };
        valueMinus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Value -= 10;
            UpdateExpandedInfo();
        };
        cycleState.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Mode = pb.Mode is ProgressBarMode.Normal ? ProgressBarMode.Marquee : ProgressBarMode.Normal;
            UpdateExpandedInfo();
        };
        toggleMode.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.State = pb.State switch
            {
                ProgressBarState.Error => ProgressBarState.Normal,
                ProgressBarState.Normal => ProgressBarState.Paused,
                ProgressBarState.Paused => ProgressBarState.Error,
                _ => throw new ArgumentException()
            };
            UpdateExpandedInfo();
        };
        intervalPlus1.Clicked += (_, e) =>
        {
            e.Cancel = true;
            ++pb.MarqueeInterval;
            UpdateExpandedInfo();
        };
        intervalMinus1.Clicked += (_, e) =>
        {
            e.Cancel = true;
            --pb.MarqueeInterval;
            UpdateExpandedInfo();
        };

        _ = new Dialog(page).Show();

        void UpdateExpandedInfo()
        {
            var ushortMax = ushort.MaxValue - ushort.MaxValue % 10;

            minPlus10.IsEnabled = pb.Minimum < ushortMax;
            minMinus10.IsEnabled = pb.Minimum > 0;
            maxPlus10.IsEnabled = pb.Maximum < ushortMax;
            maxMinus10.IsEnabled = pb.Maximum > 0;
            intervalMinus1.IsEnabled = pb.MarqueeInterval > 1;

            page.Expander.Text = $@"
{nameof(pb.Mode)} = {pb.Mode}
{nameof(pb.State)} = {pb.State}
{nameof(pb.Minimum)} = {pb.Minimum}
{nameof(pb.Maximum)} = {pb.Maximum}
{nameof(pb.Value)} = {pb.Value}
{nameof(pb.MarqueeInterval)} = {pb.MarqueeInterval}";
        }
    }

    [Test]
    public void TestRadioButtons()
    {
        using Page page = new()
        {
            WindowTitle = nameof(TestRadioButtons),
            Content = "Assert that the radio buttons are displayed properly.",
            RadioButtons = new(DefaultRadioButton.First)
            {
                "Radio #1 (default)",
                "Radio #2",
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false },
            },
        };
        _ = new Dialog(page).Show();
    }
}