using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A distance unit.</summary>
public enum DistanceUnit
{
    /// <summary>Pixel.</summary>
    Pixel,

    /// <summary>Dialog unit.</summary>
    DLU
}

/// <summary>The sizing strategy of a dialog window.</summary>
/// <remarks>This class cannot be inherited.</remarks>
public sealed class Sizing : DialogControl<PageUpdateInfo>
{
    private readonly bool _sizeToContent;

    private readonly uint _width;

    private Sizing(bool sizeToContent, uint width) => (_sizeToContent, _width) = (sizeToContent, width);

    /// <summary>The size of the window will be computed automatically.</summary>
    public static Sizing Automatic { get; } = new(false, 0);

    /// <summary>
    /// The size of the window will be computed based on the content area, similar to the message box sizing
    /// behavior.
    /// </summary>
    public static Sizing Content { get; } = new(true, 0);

    /// <summary>Creates a new <see cref="Sizing"/> object with the specified width.</summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="unit">The distance unit to use.</param>
    /// <returns>A new <see cref="Sizing"/> object.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> is negative.</exception>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="unit"/> is not a defined <see cref="DistanceUnit"/> value.
    /// </exception>
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
            _ => throw unit.InvalidEnumArgumentException()
        }));
    }

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        config.dwFlags.SetFlag(TDF_SIZE_TO_CONTENT, _sizeToContent);
        config.cxWidth = _width;
    }
}