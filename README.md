# Dialogs
[![Code Climate](https://codeclimate.com/github/5cover/Dialogs.png)](https://codeclimate.com/github/5cover/Dialogs.png)
[![CodeFactor](https://www.codefactor.io/repository/github/5cover/dialogs/badge)](https://www.codefactor.io/repository/github/5cover/dialogs)

Managed Win32 [task dialog](https://learn.microsoft.com/en-us/windows/win32/controls/task-dialogs-overview) wrapper.
Supports all native Task Dialog features.

# Information

In order to use activation contexts, this package copies the [XPThemes.manifest](https://github.com/5cover/Dialogs/blob/master/Dialogs/XPThemes.manifest) file to the output directory.

# Usage

### Simple
Shows a simple task dialog.
```cs
using Scover.Dialogs;

using Page page = new()
{
    Content = "Sample text",
    Buttons = { Button.Yes, Button.No, Button.Cancel }
};

var clickedButton = new Dialog(page).Show();
```

### Multi-page
```cs
using Scover.Dialogs;

using Page page1 = new()
{
    MainInstruction = "Page #1",
    Buttons = new(ButtonStyle.CommandLinks)
    {
        { "Label", "Supplemental instruction" },
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
    [page1] = request => request.Kind is NavigationRequestKind.Cancel ? null : page2,
}).Show();
```

Check out [Tests.cs](https://github.com/5cover/Dialogs/blob/master/Tests/DialogTests.cs) for more examples.

