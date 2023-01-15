# Dialogs
[NuGet](https://www.nuget.org/packages/Scover.Dialogs)

Managed Win32 [task dialog](https://learn.microsoft.com/en-us/windows/win32/controls/task-dialogs-overview) wrapper.
Supports all native Task Dialog features.

# Usage

### Simple
Shows a simple task dialog.
```cs
using Scover.Dialogs;

using Page page = new()
{
    Content = "Sample text",
    Buttons = { Button.Yes, Button.No }
};

var clickedButton = new Dialog(page).Show();
```

### Multi-page
```cs
using Scover.Dialogs;

using Page page1 = new()
{
    MainInstruction = "Page #1",
    Buttons =
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
    [page1] = _ => page2,
}).Show();
```

Check out [Tests.cs](https://github.com/5cover/Dialogs/blob/master/Tests/DialogTests.cs) for more examples.

