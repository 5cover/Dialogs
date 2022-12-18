using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>The sizing strategy of a dialog window.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class Sizing : DialogControl<PageUpdateInfo>
{
    /// <summary>The size of the window will be computed automatically.</summary>
    public const Sizing? Automatic = null;

    private readonly bool _sizeToContent;
    private readonly uint _width;

    private Sizing(bool sizeToContent, uint width) => (_sizeToContent, _width) = (sizeToContent, width);

    /// <summary>The size of the window will be computed based on the content area, similar to the message box sizing behavior.</summary>
    public static Sizing Content { get; } = new(true, 0);

    /// <summary>Creates a new <see cref="Sizing"/> object with the specified width.</summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="unit">The distance unit to use.</param>
    /// <returns>A new <see cref="Sizing"/> object.</returns>
    public static Sizing FromWidth(int width, DistanceUnit unit = DistanceUnit.DLU)
    {
        if (width < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "The value is less than zero.");
        }
        return new(false, (uint)(unit switch
        {
            DistanceUnit.Pixel => width * 4 / Macros.SignedLOWORD((int)User32.GetDialogBaseUnits()),
            DistanceUnit.DLU => width,
            _ => throw unit.NewInvalidEnumArgumentException(nameof(unit))
        }));
    }

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        config.dwFlags.SetFlag(TASKDIALOG_FLAGS.TDF_SIZE_TO_CONTENT, _sizeToContent);
        config.cxWidth = _width;
    }
}