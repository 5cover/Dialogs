using System.Text;
using Scover.Dialogs;
using Scover.Dialogs.Parts;

internal static class Program
{
    private static DialogIcon GetRandomIcon()
    {
        int id = 199;
        int c = 0;
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
            ++c;
            id = Random.Shared.Next(2, 6401);
        }
        Console.WriteLine($"c = {c}, id = {id}");
        return DialogIcon.FromId(id);
    }

    private static string GetRandomString(int maxLength = 200)
    {
        Random random = new();
        var plainText = new StringBuilder();
        var length = random.Next(10, maxLength);
        Console.WriteLine($"Length = {length}");
        for (; length > 0; --length)
        {
            // Avoid CJK ranges becuase CJK ranges characters are often shown when layout fails.
            _ = plainText.Append((char)random.Next(char.MinValue, '\u2E80'));
        }

        return plainText.ToString();
    }

    private static void Main()
    {
        Dialog.UseActivationContext = false;
    }

    private static void TestOuttaControl()
    {
        using Page page = new()
        {
            IsCancelable = true,
            Expander = new()
            {
                CollapseButtonText = GetRandomString(40),
                ExpandButtonText = GetRandomString(40),
                IsExpanded = true
            },
            Buttons = new CommandLinkCollection()
            {
                { GetRandomString(40), GetRandomString() },
                { GetRandomString(40), GetRandomString() },
            },
            RadioButtons = new()
            {
                GetRandomString(40),
                GetRandomString(40),
            },
            WindowTitle = GetRandomString(40),
            Verification = new(GetRandomString()),
            FooterText = " ",
        };
        page.Created += async (s, e) =>
        {
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(1000));
            while (await timer.WaitForNextTickAsync())
            {
                page.Content = GetRandomString();
                page.MainInstruction = GetRandomString();
                page.Expander.Text = GetRandomString();
                page.FooterText = GetRandomString();
                page.Icon = GetRandomIcon();
                page.FooterIcon = GetRandomIcon();
            }
            page.Close();
        };

        _ = new Dialog(page).Show();
    }
}