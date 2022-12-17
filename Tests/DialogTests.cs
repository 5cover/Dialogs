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
    public void TestProgressBar()
    {
        Button
            minAdd10 = new("Min + 10"),
            minSubstract10 = new("Min - 10"),
            maxAdd10 = new("Max + 10"),
            maxSubstract10 = new("Max - 10"),
            valueAdd10 = new("Value + 10"),
            valueSubstract10 = new("Value - 10");
        using Page page = new()
        {
            Content = "Assert that the progress bar is displayed properly.",
            Buttons = new ButtonCollection()
            {
                minAdd10,
                minSubstract10,
                maxAdd10,
                maxSubstract10,
                valueAdd10,
                valueSubstract10,
            },
            Expander = new() { IsExpanded = true, ExpandedInformation = "in ur face" },
            IsCancelable = true,
            ProgressBar = new(),
            WindowTitle = nameof(TestProgressBar),
        };

        var pb = page.ProgressBar;

        EnableDisable();
        UpdateExpandedInfo();

        page.ButtonClicked += (s, e) =>
        {
            e.Cancel = true;
            if (minAdd10.Equals(e.ClickedControl))
            {
                pb.Minimum += 10;
            }
            else if (minSubstract10.Equals(e.ClickedControl))
            {
                pb.Minimum -= 10;
            }
            else if (maxAdd10.Equals(e.ClickedControl))
            {
                pb.Maximum += 10;
            }
            else if (maxSubstract10.Equals(e.ClickedControl))
            {
                pb.Maximum -= 10;
            }
            else if (valueAdd10.Equals(e.ClickedControl))
            {
                pb.Value += 10;
            }
            else if (valueSubstract10.Equals(e.ClickedControl))
            {
                pb.Value -= 10;
            }
            else
            {
                e.Cancel = false;
            }

            EnableDisable();
            UpdateExpandedInfo();
        };
        _ = new Dialog(page).Show();

        void UpdateExpandedInfo() => page.Expander.ExpandedInformation = @$"{nameof(ProgressBar.Minimum)} = {page.ProgressBar.Minimum}
{nameof(ProgressBar.Maximum)} = {page.ProgressBar.Maximum}
{nameof(ProgressBar.Value)} = {page.ProgressBar.Value}";

        void EnableDisable()
        {
            minAdd10.IsEnabled = pb.Minimum < ushort.MaxValue;
            minSubstract10.IsEnabled = pb.Minimum > 0;
            maxAdd10.IsEnabled = pb.Maximum < ushort.MaxValue;
            maxSubstract10.IsEnabled = pb.Maximum > 0;
            valueAdd10.IsEnabled = pb.Value < ushort.MaxValue;
            valueSubstract10.IsEnabled = pb.Value > 0;
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