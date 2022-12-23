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
        RadioButton? selectedRadioButton = new("Radio #1");
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
                selectedRadioButton,
                "Radio #2",
            },
            WindowTitle = nameof(TestClick),
        };
        page.Created += (s, e) =>
        {
            selectedRadioButton.Click();
            foreach (CommitControl cc in page.Buttons)
            {
                clickedControl = cc;
                cc.Click();
            }
            page.Close();
        };
        foreach (var cc in page.Buttons)
        {
            cc.Clicked += (s, e) =>
            {
                Assert.That(s, Is.EqualTo(clickedControl));
                e.Cancel = true;
            };
        }
        clickedControl = new Dialog(page).Show();
        Assert.That(clickedControl, Is.EqualTo(Button.Cancel));
    }

    [Test]
    public void TestCloseImmediately()
    {
        using Page page = new();
        page.Created += (s, e) => page.Close();
        _ = new Dialog(page).Show();
    }
}