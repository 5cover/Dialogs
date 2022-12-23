using Scover.Dialogs;

using Page page1 = new()
{
    MainInstruction = "Page #1",
    Buttons = new CommandLinkCollection()
    {
      { "Label", "Supplental instruction" },
      Button.Cancel
    }
};
using Page page2 = new()
{
    MainInstruction = "Page #2",
    Expander = new("Expanded information")
};

var clickedButton = new MultiPageDialog(page1, new Dictionary<Page, NextPageSelector>
{
    [page1] = clickedControl => Button.Cancel.Equals(clickedControl) ? null : page2,
}).Show();