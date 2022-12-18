using NUnit.Framework;
using Scover.Dialogs;
using Scover.Dialogs.Parts;

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
            AreHyperlinksEnabled = true,
            IsCancelable = true,
            IsMinimizable = true,
            IsRightToLeftLayout = false,
            Buttons = new ButtonCollection(button2)
            {
                "Button #1",
                button2,
                new Button("Button #3 (Admin)") { RequiresElevation = true },
                new Button("Button #4 (Disabled)") { IsEnabled = false },
                new Button("Button #5 (Admin && Disabled)") { RequiresElevation = true, IsEnabled = false }
            },
            Expander = new Expander()
            {
                ExpandedInformation = nameof(Expander.ExpandedInformation),
                CollapseButtonText = nameof(Expander.CollapseButtonText),
                ExpandButtonText = nameof(Expander.ExpandButtonText),
                ExpanderPosition = ExpanderPosition.BelowContent,
                IsExpanded = true
            },
            FooterIcon = DialogIcon.Information,
            Icon = DialogIcon.ShieldWarningYellowBar,
            ProgressBar = new ProgressBar() { Maximum = 90, Minimum = 80, Value = 83 },
            RadioButtons = new RadioButtonCollection()
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
            Buttons = new CommandLinkCollection(commandLink2)
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
    public void TestIcon()
    {
        ushort index = 0;
        using var hIcon = Vanara.PInvoke.Shell32.ExtractAssociatedIcon(default, new(@"C:\Windows\System32\rstrui.exe"), ref index);
        DialogIcon customIcon = DialogIcon.FromHandle(hIcon.DangerousGetHandle());

        var buttonContinue = Button.Continue;
        using Page page = new()
        {
            Buttons = new ButtonCollection()
            {
                buttonContinue,
                Button.Close
            },
            Content = "Assert that the icon is displayed properly.",
            Icon = DialogIcon.Error,
            WindowTitle = nameof(TestIcon),
        };
        buttonContinue.Clicked += (s, e) =>
        {
            if (page.Icon == DialogIcon.Error)
            {
                page.Icon = DialogIcon.Information;
            }
            if (page.Icon == DialogIcon.Information)
            {
                page.Icon = customIcon;
            }
            if (page.Icon == customIcon)
            {
                page.Icon = DialogIcon.None;
            }
            e.Cancel = true;
        };
        _ = new Dialog(page).Show();
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
            toggleMode = new("Toggle mode"),
            cycleState = new("Cycle state"),
            intervalPlus1 = new("Interval + 1"),
            intervalMinus1 = new("Interval - 1");
        using Page page = new()
        {
            Content = "Assert that the progress bar behaves properly.",
            Buttons = new ButtonCollection()
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

        EnableDisable();
        UpdateExpandedInfo();

        page.ButtonClicked += (s, e) =>
        {
            e.Cancel = true;
            if (minPlus10.Equals(e.ClickedControl))
            {
                pb.Minimum += 10;
            }
            else if (minMinus10.Equals(e.ClickedControl))
            {
                pb.Minimum -= 10;
            }
            else if (maxPlus10.Equals(e.ClickedControl))
            {
                pb.Maximum += 10;
            }
            else if (maxMinus10.Equals(e.ClickedControl))
            {
                pb.Maximum -= 10;
            }
            else if (valuePlus10.Equals(e.ClickedControl))
            {
                pb.Value += 10;
            }
            else if (valueMinus10.Equals(e.ClickedControl))
            {
                pb.Value -= 10;
            }
            else if (toggleMode.Equals(e.ClickedControl))
            {
                pb.Mode = pb.Mode is ProgressBarMode.Normal ? ProgressBarMode.Marquee : ProgressBarMode.Normal;
            }
            else if (cycleState.Equals(e.ClickedControl))
            {
                pb.State = pb.State switch
                {
                    ProgressBarState.Error => ProgressBarState.Normal,
                    ProgressBarState.Normal => ProgressBarState.Paused,
                    ProgressBarState.Paused => ProgressBarState.Error,
                    _ => throw new ArgumentException()
                };
            }
            else if (intervalPlus1.Equals(e.ClickedControl))
            {
                pb.MarqueeInterval++;
            }
            else if (intervalMinus1.Equals(e.ClickedControl))
            {
                pb.MarqueeInterval--;
            }
            else
            {
                e.Cancel = false;
            }

            EnableDisable();
            UpdateExpandedInfo();
        };
        _ = new Dialog(page).Show();

        void UpdateExpandedInfo() => page.Expander.ExpandedInformation = $@"
{nameof(pb.Mode)} = {pb.Mode}
{nameof(pb.State)} = {pb.State}
{nameof(pb.Minimum)} = {pb.Minimum}
{nameof(pb.Maximum)} = {pb.Maximum}
{nameof(pb.Value)} = {pb.Value}
{nameof(pb.MarqueeInterval)} = {pb.MarqueeInterval}";

        void EnableDisable()
        {
            minPlus10.IsEnabled = pb.Minimum < ushort.MaxValue;
            minMinus10.IsEnabled = pb.Minimum > 0;
            maxPlus10.IsEnabled = pb.Maximum < ushort.MaxValue;
            maxMinus10.IsEnabled = pb.Maximum > 0;
            intervalMinus1.IsEnabled = pb.MarqueeInterval > 1;
        }
    }

    [Test]
    public void TestRadioButtons()
    {
        RadioButton radio2 = new("Radio #2 (default)");
        using Page page = new()
        {
            Content = "Assert that the radio buttons are displayed properly.",
            Buttons = new ButtonCollection()
            {
                Button.OK,
                Button.Cancel
            },
            RadioButtons = new(radio2)
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