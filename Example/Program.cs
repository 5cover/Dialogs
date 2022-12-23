using Scover.Dialogs;

internal static class Program
{
    public static void TestNavigation()
    {
        using Page page1 = new()
        {
            MainInstruction = "Page 1",
            Content = "This is the first page with expander",
            Expander = new("Expanded information")
            {
                ExpandButtonText = "Custom expand",
                CollapseButtonText = "Custom collapse",
                IsExpanded = true,
            }
        };
        using Page page2 = new()
        {
            MainInstruction = "Page 2",
            Content = "This is the second page with radio buttons",
            RadioButtons = new(DefaultRadioButton.None)
            {
                "Radio #1",
                "Radio #2"
            }
        };
        using Page page3 = new()
        {
            MainInstruction = "Page 3",
            Content = "This is the third page with nothing at all"
        };
        _ = new Dialog(page1, (page, cc) =>
        {
            if (page == page1)
            {
                return page2;
            }
            if (page == page2)
            {
                return page3;
            }
            return null;
        }).Show();
    }

    private static void Main()
    {
        Dialog.UseActivationContext = false;
        TestNavigation();
    }
}