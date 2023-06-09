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
        ButtonBase? clickedButton = null;
        RadioButton? selectedRadioButton = new("Radio #1");
        using Page page = new()
        {
            WindowTitle = nameof(TestClick),
            //RadioButtons = { selectedRadioButton, "Radio #2", },
            Buttons = { Button.OK, "Custom #1", "Custom #2", },
        };
        page.Created += (s, e) =>
        {
            selectedRadioButton.Click();
            foreach (ButtonBase b in page.Buttons)
            {
                clickedButton = b;
                b.Clicked += (s, e) =>
                {
                    Assert.That(s, Is.EqualTo(clickedButton));
                    e.Cancel = true;
                };
                b.Click();
            }
            page.Exit();
        };
        Assert.That(new Dialog(page).Show(), Is.Null);
    }

    [Test]
    public void TestCloseImmediately()
    {
        using Page page = new();
        page.Created += (s, e) => page.Exit();
        Assert.That(new Dialog(page).Show(), Is.Null);
    }

    [Test]
    public void TestDynamic()
    {
        using Page page = new()
        {
            WindowTitle = GetRandomString(20),
            RadioButtons = { GetRandomString(40), GetRandomString(40), },
            Expander = new()
            {
                CollapseButtonText = GetRandomString(10),
                ExpandButtonText = GetRandomString(10),
                IsExpanded = true
            },
            Verification = new(GetRandomString()),
            Buttons = new(ButtonStyle.CommandLinks)
            {
                { GetRandomString(40), GetRandomString() },
            },
            FooterText = " ",
        };
        page.Created += async (s, e) =>
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));
            int tickCount = 0;
            while (await timer.WaitForNextTickAsync() && tickCount < 5)
            {
                page.Icon = GetRandomIcon();
                page.Content = GetRandomString();
                page.FooterIcon = GetRandomIcon();
                page.FooterText = GetRandomString();
                page.Expander.Text = GetRandomString();
                page.MainInstruction = GetRandomString();
                ++tickCount;
            }
            page.Exit();
        };
        _ = new Dialog(page).Show();
    }

    private static DialogIcon GetRandomIcon()
    {
        int id;
        do
        {
            id = Random.Shared.Next(2, 6401);
        } while (id
            is > 198 and < 1001
            or > 1043 and < 1301
            or > 1306 and < 1400
            or > 1405 and < 5100
            or > 5102 and < 5201
            or > 5206 and < 5210
            or > 5210 and < 5301
            or > 5412 and < 6400);
        return DialogIcon.FromId(id);
    }

    private static string GetRandomString(int maxLength = 100)
    {
        var plainText = new StringBuilder();
        for (var length = Random.Shared.Next(10, maxLength); length > 0; --length)
        {
            _ = plainText.Append((char)Random.Shared.Next(char.MinValue, '\u2E80'));
        }
        return plainText.ToString();
    }
}