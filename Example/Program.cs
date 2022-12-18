// See https://aka.ms/new-console-template for more information
using Scover.Dialogs;
using Scover.Dialogs.Parts;
using Vanara.PInvoke;

internal class Program
{
    private static void Main(string[] args)
    {
        Dialog.UseActivationContext = false;
        MegaTest();
    }

    private static void MegaTest()
    {
        Button button2 = new("Button #2");
        using Page page = new()
        {
            AreHyperlinksEnabled = true,
            IsCancelable = true,
            IsMinimizable = true,
            IsRightToLeftLayout = false,
            Buttons = new ButtonCollection(button2)
            {
                "Button #1",
                button2,
                new Button("Button #3 (Admin)") { RequiresElevation = true },
                new Button("Button #4 (Disabled)") { IsEnabled = false },
                new Button("Button #5 (Admin && Disabled)") { RequiresElevation = true, IsEnabled = false }
            },
            Expander = new Expander()
            {
                ExpandedInformation = nameof(Expander.ExpandedInformation),
                CollapseButtonText = nameof(Expander.CollapseButtonText),
                ExpandButtonText = nameof(Expander.ExpandButtonText),
                ExpanderPosition = ExpanderPosition.BelowContent,
                IsExpanded = true
            },
            FooterIcon = DialogIcon.Information,
            Icon = DialogIcon.ShieldWarningYellowBar,
            ProgressBar = new ProgressBar() { Maximum = 90, Minimum = 80, Value = 83 },
            RadioButtons = new RadioButtonCollection()
            {
                "Radio #1",
                "Radio #2",
                new RadioButton("Radio #3 (disabled)") { IsEnabled = false }
            },
            Sizing = Sizing.FromWidth(500, DistanceUnit.Pixel),
            Content = $"{nameof(page.Content)} with <A>hyperlink</A>. Assert that the page is displayed properly.",
            FooterText = nameof(page.FooterText),
            MainInstruction = nameof(page.MainInstruction),
            Verification = new(nameof(page.Verification)) { IsChecked = true },
            WindowTitle = nameof(MegaTest),
        };
        Dialog dlg = new(page);
        var r = dlg.Show();
        Console.WriteLine(r is Button b ? b.Text : (r?.ToString() ?? "null"));
    }

    private static void NativeTest() => ComCtl32.TaskDialog(HWND.NULL,
                                           HINSTANCE.NULL,
                                           "Test",
                                           "Main instruction",
                                           "Content",
                                           ComCtl32.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON,
                                           new((nint)ComCtl32.TaskDialogIcon.TD_SECURITYSUCCESS_ICON),
                                           out _).ThrowIfFailed();
}