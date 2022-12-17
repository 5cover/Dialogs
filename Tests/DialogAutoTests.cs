using NUnit.Framework;
using Scover.Dialogs;
using Scover.Dialogs.Parts;

namespace Tests;

[TestFixture, Timeout(1000)]
public sealed class DialogAutoTests
{
    [Test]
    public void TestClick()
    {
        CommitControl? clickedControl = null;
        RadioButton? selectedRadioButton = null;
        using Page page = new()
        {
            Content = "Assert that the correct buttons get clicked",
            Buttons = new ButtonCollection()
            {
                Button.OK,
                Button.Cancel,
                "Custom #1",
                "Custom #2",
            },
            RadioButtons = new RadioButtonCollection()
            {
                "Radio #1",
                "Radio #2",
            },
            WindowTitle = nameof(TestClick),
        };
        page.Created += (s, e) =>
        {
            selectedRadioButton = page.RadioButtons.First();
            selectedRadioButton.Click();
            foreach (CommitControl cc in page.Buttons)
            {
                clickedControl = cc;
                cc.Click();
            }
            page.Close();
        };
        page.ButtonClicked += (s, e) =>
        {
            Assert.That(e.ClickedControl, Is.EqualTo(clickedControl));
            e.Cancel = true;
        };

        var result = new Dialog(page).Show();

        Assert.Multiple(() =>
        {
            Assert.That(result.ClickedControl, Is.EqualTo(Button.Cancel));
            Assert.That(result.SelectedRadioButton, Is.EqualTo(selectedRadioButton));
        });
    }

    [Test]
    public void TestCloseImmediately()
    {
        using Page page = new();
        page.Created += (s, e) => page.Close();
        _ = new Dialog(page).Show();
    }
}