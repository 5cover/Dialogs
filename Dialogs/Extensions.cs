using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Vanara.Extensions;
using Vanara.PInvoke;

namespace Scover.Dialogs;

internal static class Extensions
{
    /// <inheritdoc cref="INativeProvider{TNative}.GetNative"/>
    public static TNative GetNative<TNative>(this INativeProvider<TNative> np) => np.GetNative();

    [StackTraceHidden]
    public static InvalidEnumArgumentException NewInvalidEnumArgumentException<TEnum>(this TEnum value, string? argumentName = null) where TEnum : struct, Enum, IConvertible
        => new(argumentName, value.ToInt32(CultureInfo.InvariantCulture), typeof(TEnum));

    public static void Raise(this EventHandler? @event, object sender) => @event?.Invoke(sender, EventArgs.Empty);

    public static nint SendMessage<TMsg, TWP, TLP>(this HWND hwnd, TMsg msg, TWP wParam, TLP lParam) where TMsg : IConvertible where TWP : IConvertible where TLP : IConvertible
            => hwnd.SendMessage(msg, wParam.ToNint(), lParam.ToNint());

    public static nint SendMessage<TMsg, TWP>(this HWND hwnd, TMsg msg, TWP wParam, nint lParam = default) where TMsg : IConvertible where TWP : IConvertible
        => hwnd.SendMessage(msg, wParam.ToNint(), lParam);

    public static nint SendMessage<TMsg>(this HWND hwnd, TMsg msg, nint wParam, nint lParam) where TMsg : IConvertible
        => User32.SendMessage(hwnd, msg.ToUInt32(CultureInfo.InvariantCulture), wParam, lParam);

    public static void SetFlag<TEnum>(this ref TEnum value, TEnum flag, bool isSet) where TEnum : unmanaged, Enum
            => EnumExtensions.SetFlags(ref value, flag, isSet);

    private static nint ToNint<T>(this T val) where T : IConvertible => (nint)val.ToInt64(CultureInfo.InvariantCulture);
}