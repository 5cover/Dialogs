using NUnit.Framework;
using Scover.Dialogs;

namespace Tests;

[TestFixture]
public sealed class DialogTests
{
    static DialogTests()
    {
        Dialog.UseActivationContext = true;
    }

    [Test]
    public void MegaTest()
    {
        Button button2 = new("Button #2");
        using Page page = new()
        {
            AllowHyperlinks = true,
            IsCancelable = true,
            IsMinimizable = true,
            IsRightToLeftLayout = false,
            Buttons = new(defaultItem: button2)
            {
                "Button #1",
                button2,
                new Button("Button #3 (Admin)") { RequiresElevation = true },
                new Button("Button #4 (Disabled)") { IsEnabled = false },
                new Button("Button #5 (Admin && Disabled)") { RequiresElevation = true, IsEnabled = false }
            },
            Expander = new Expander()
            {
                Text = nameof(Expander.Text),
                CollapseButtonText = nameof(Expander.CollapseButtonText),
                ExpandButtonText = nameof(Expander.ExpandButtonText),
                ExpanderPosition = ExpanderPosition.BelowContent,
                IsExpanded = true
            },
            FooterIcon = DialogIcon.Information,
            Header = DialogHeader.Yellow,
            Icon = DialogIcon.ErrorShield,
            ProgressBar = new ProgressBar() { Maximum = 90, Minimum = 80, Value = 83 },
            RadioButtons =
            {
                "Radio #1",
                "Radio #2",
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false }
            },
            Sizing = Sizing.FromWidth(500, DistanceUnit.Pixel),
            Content = $"{nameof(page.Content)} with <A>hyperlink</A>. Assert that the page is displayed properly.",
            FooterText = nameof(page.FooterText),
            MainInstruction = nameof(page.MainInstruction),
            Verification = new(nameof(page.Verification)) { IsChecked = true },
            WindowTitle = nameof(MegaTest),
        };
        Dialog dlg = new(page);
        _ = dlg.Show();
    }

    [Test]
    public void TestCommandLinks()
    {
        CommandLink commandLink2 = new("Command link #2 (default)", "This is the default command link.");
        using Page page = new()
        {
            Content = "Assert that the command links are displayed properly.",
            Buttons = new(defaultItem: commandLink2, style: CommitControlStyle.CommandLinks)
            {
                "Command link #1",
                commandLink2,
                { "Command link #3", "Supplemental explanation" },
                new CommandLink("Command link #4 (disabled)") { IsEnabled = false },
                new CommandLink("Command link #5 (admin)") { RequiresElevation = true },
                new CommandLink("Command link #6 (admin && disabled)") { IsEnabled = false, RequiresElevation = true },

                // Common buttons are also supported.
                Button.Abort,
                Button.Help,
            },
            WindowTitle = nameof(TestCommandLinks),
        };
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestCustomIcon()
    {
        ushort index = 0;
        using var hIcon = Vanara.PInvoke.Shell32.ExtractAssociatedIcon(default, new(@"C:\Windows\System32\rstrui.exe"), ref index);
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
            Buttons = { buttonNextIcon, Button.Close, },
            Content = GetContent("information"),
            Header = DialogHeader.Green,
            Icon = DialogIcon.Information,
            MainInstruction = "Main instruction",
            Verification = new("Main instruction"),
            WindowTitle = nameof(TestIcon),
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
    public void TestNavigation()
    {
        using Page page1 = new()
        {
            MainInstruction = "Page 1",
            Buttons = { Button.Yes, Button.No },
            Content = "First page with expander. Press F1 to navigate to Page 2.",
            Expander = new("Expanded information")
            {
                ExpandButtonText = "Custom expand",
                CollapseButtonText = "Custom collapse",
                IsExpanded = true,
            },
            IsCancelable = true,
        };
        using Page page2 = new()
        {
            MainInstruction = "Page 2",
            Content = "Second page with radio buttons. Press F1 to navigate to Page 3",
            RadioButtons = new(defaultItem: DefaultRadioButton.None)
            {
                "Radio #1",
                "Radio #2"
            },
            IsCancelable = true,
        };
        using Page page3 = new()
        {
            MainInstruction = "Page 3",
            Content = "Third page with nothing at all. Press F1 to navigate to Page 1.",
            IsCancelable = true,
        };
        MultiPageDialog dlg = new(page1, new Dictionary<Page, NextPageSelector>
        {
            [page1] = _ => page2,
            [page2] = _ => page3,
            [page3] = _ => page1,
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
            Content = "Assert that the progress bar behaves properly.",
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
                intervalMinus1
            },
            Sizing = Sizing.FromWidth(200),
            Expander = new() { IsExpanded = true },
            IsCancelable = true,
            IsMinimizable = true,
            ProgressBar = new(),
            WindowTitle = nameof(TestProgressBar),
        };
        var pb = page.ProgressBar;
        UpdateExpandedInfo();
        minPlus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Minimum += 10;
            minPlus10.IsEnabled = pb.Minimum < ushort.MaxValue;
            UpdateExpandedInfo();
        };
        minMinus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Minimum -= 10;
            minMinus10.IsEnabled = pb.Minimum > 0;
            UpdateExpandedInfo();
        };
        maxPlus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Maximum += 10;
            maxPlus10.IsEnabled = pb.Maximum < ushort.MaxValue;
            UpdateExpandedInfo();
        };
        maxMinus10.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.Maximum -= 10;
            maxMinus10.IsEnabled = pb.Maximum > 0;
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
            pb.MarqueeInterval++;
            UpdateExpandedInfo();
        };
        intervalMinus1.Clicked += (_, e) =>
        {
            e.Cancel = true;
            pb.MarqueeInterval--;
            intervalMinus1.IsEnabled = pb.MarqueeInterval > 1;
            UpdateExpandedInfo();
        };
        _ = new Dialog(page).Show();

        void UpdateExpandedInfo() => page.Expander.Text = $@"
{nameof(pb.Mode)} = {pb.Mode}
{nameof(pb.State)} = {pb.State}
{nameof(pb.Minimum)} = {pb.Minimum}
{nameof(pb.Maximum)} = {pb.Maximum}
{nameof(pb.Value)} = {pb.Value}
{nameof(pb.MarqueeInterval)} = {pb.MarqueeInterval}";
    }

    [Test]
    public void TestProgressBarNavigation()
    {
        using Page page1 = new()
        {
            Content = "This is the first page.",
            WindowTitle = nameof(TestProgressBarNavigation)
        };
        using Page page2 = new()
        {
            Content = "This is the second page. Assert that the progress bar is shown properly. The bar should be half-filled.",
            ProgressBar = new()
            {
                Value = 5,
                Maximum = 10,
            },
            WindowTitle = nameof(TestProgressBarNavigation)
        };
        _ = new MultiPageDialog(page1, new Dictionary<Page, NextPageSelector>() { [page1] = _ => page2 }).Show();
    }

    [Test]
    public void TestRadioButtons()
    {
        RadioButton radio2 = new("Radio #2 (default)");
        using Page page = new()
        {
            Content = "Assert that the radio buttons are displayed properly.",
            RadioButtons = new(defaultItem: radio2)
            {
                "Radio #1",
                radio2,
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false },
            },
            WindowTitle = nameof(TestRadioButtons),
        };
        _ = new Dialog(page).Show();
    }
}