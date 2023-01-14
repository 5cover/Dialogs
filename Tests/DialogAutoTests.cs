using System.Text;
using NUnit.Framework;
using Scover.Dialogs;

namespace Tests;

[TestFixture, Timeout(10000)]
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
            Buttons = { Button.OK, "Custom #1", "Custom #2", },
            RadioButtons = { selectedRadioButton, "Radio #2", },
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
        Assert.That(clickedControl, Is.Null);
    }

    [Test]
    public void TestCloseImmediately()
    {
        using Page page = new();
        page.Created += (s, e) => page.Close();
        _ = new Dialog(page).Show();
    }

    [Test]
    public void TestDynamic()
    {
        using Page page = new()
        {
            Expander = new()
            {
                CollapseButtonText = GetRandomString(40),
                ExpandButtonText = GetRandomString(40),
                IsExpanded = true
            },
            Buttons =
            {
                { GetRandomString(40), GetRandomString() },
                { GetRandomString(40), GetRandomString() },
            },
            RadioButtons = { GetRandomString(40), GetRandomString(40), },
            WindowTitle = GetRandomString(40),
            Verification = new(GetRandomString()),
            FooterText = " ",
        };
        page.Created += async (s, e) =>
        {
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500));
            int tickCount = 0;
            while (await timer.WaitForNextTickAsync() && tickCount < 5)
            {
                page.Content = GetRandomString();
                page.MainInstruction = GetRandomString();
                page.Expander.Text = GetRandomString();
                page.FooterText = GetRandomString();
                page.Icon = GetRandomIcon();
                page.FooterIcon = GetRandomIcon();
                ++tickCount;
            }
            page.Close();
        };
        _ = new Dialog(page).Show();

        static DialogIcon GetRandomIcon()
        {
            int id = 199;
            while (id
                is > 198 and < 1001
                or > 1043 and < 1301
                or > 1306 and < 1400
                or > 1405 and < 5100
                or > 5102 and < 5201
                or > 5206 and < 5210
                or > 5210 and < 5301
                or > 5412 and < 6400)
            {
                id = Random.Shared.Next(2, 6401);
            }
            return DialogIcon.FromId(id);
        }

        static string GetRandomString(int maxLength = 200)
        {
            Random random = new();
            var plainText = new StringBuilder();
            for (var length = random.Next(10, maxLength); length > 0; --length)
            {
                _ = plainText.Append((char)random.Next(char.MinValue, '\u2E80'));
            }
            return plainText.ToString();
        }
    }
}