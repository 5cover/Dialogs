using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

internal sealed class Program
{
    private static void Main()
    {
        using TASKDIALOGCONFIG config = new()
        {
            dwCommonButtons = TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON,
            pfCallbackProc = (hwnd, id, wParam, lParam, refData) =>
            {
                Console.WriteLine(id);
                return default;
            }
        };
        _ = TaskDialogIndirect(config, out var clickedButtonId, out var selectedRadioButtonId, out var isVerificationChecked);
        Console.WriteLine($"clicked button : {clickedButtonId} ({(User32.MB_RESULT)clickedButtonId})");
    }
}