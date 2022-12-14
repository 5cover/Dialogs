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
        using Page page = new()
        {
            AreHyperlinksEnabled = true,
            IsCancelable = true,
            IsMinimizable = true,
            IsRightToLeftLayout = false,
            Buttons = new ButtonCollection()
            {
                "Button #1",
                new Button("Button #2 (Admin)") { RequiresElevation = true },
                new Button("Button #3 (Disabled)") { IsEnabled = false },
                new Button("Button #4 (Admin && Disabled)") { RequiresElevation = true, IsEnabled = false }
            },
            Expander = new Expander()
            {
                ExpandedInformation = "ExpandedInformation",
                CollapseButtonText = "Custom Collapse",
                ExpandButtonText = "Custom Expand",
                ExpanderPosition = ExpanderPosition.BelowFooter,
                IsExpanded = true
            },
            FooterIcon = DialogIcon.Information,
            Icon = DialogIcon.ShieldWarningYellowBar,
            ProgressBar = new ProgressBar() { Value = 50 },
            RadioButtons = new RadioButtonCollection()
            {
                "Radio #1",
                "Radio #2",
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false }
            },
            Sizing = Sizing.FromWidth(500, DistanceUnit.Pixel),
            Content = "Content with <A>hyperlink</A>",
            FooterText = "footer text",
            MainInstruction = "main instruction",
            Verification = new("verification text") { IsChecked = true },
            WindowTitle = nameof(MegaTest),
        };
        Dialog dlg = new(page);
        _ = dlg.Show();
    }

    [Test]
    public void TestClose()
    {
        Verification verification = new("Check to close");
        using Page page = new()
        {
            Content = "This dialog cannot be closed by normal means",
            IsCancelable = true,
            Verification = verification,
            WindowTitle = nameof(TestClose),
        };
        page.ButtonClicked += (s, e) => e.Cancel = true;
        verification.Checked += (s, e) => page.Close();
        Dialog dlg = new(page);
        _ = dlg.Show();
    }

    [Test]
    public void TestCommandLinks()
    {
        Dialog dlg = new(new()
        {
            Content = "Assert that the command links are displayed properly.",
            Buttons = new CommandLinkCollection()
            {
                "Command link #1",
                { "Command link #2", "Supplemental explanation" },
                new Button("Command link #3 (disabled)") { IsEnabled = false },
                new Button("Command link #4 (admin)") { RequiresElevation = true },
                new Button("Command link #5 (admin && disabled)") { IsEnabled = false, RequiresElevation = true },
            },
            WindowTitle = nameof(TestCommandLinks),
        });
        _ = dlg.Show();
    }

    [Test]
    public void TestEmpty()
    {
        Dialog dlg = new(new());
        _ = dlg.Show();
    }

    [Test]
    public void TestRadioButtons()
    {
        RadioButton defaultRadioButton = new("Radio #2 (default)")
        {
            IsEnabled = false
        };
        RadioButtonCollection radioButtons = new()
        {
            "Radio #1",
            defaultRadioButton,
            new RadioButton("Radio #3 (disabled)") { IsEnabled = false },
        };
        radioButtons.DefaultItem = defaultRadioButton;
        Dialog dlg = new(new()
        {
            Content = "Assert that the radio buttons are displayed properly.",
            RadioButtons = radioButtons,
            Buttons = new ButtonCollection()
            {
                Button.OK,
                Button.Cancel
            },
            WindowTitle = nameof(TestRadioButtons),
        });
        _ = dlg.Show();
    }
}