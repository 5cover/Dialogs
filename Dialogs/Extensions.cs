using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace Scover.Dialogs;

internal static class Extensions
{
    /// <summary>Asserts that <paramref name="t"/> isn't <see langword="null"/>.</summary>
    /// <returns><paramref name="t"/>, not null.</returns>
    public static T AssertNotNull<T>([NotNull] this T? t)
    {
        Debug.Assert(t is not null, $"{nameof(t)} is null.");
        return t;
    }

    /// <summary>Forwards a notification to a collection of handlers.</summary>
    /// <returns>
    /// <see langword="null"/> if none of the handlers had a meaningful value to return, the
    /// notification-specific return value of the first handler that did otherwise.
    /// </returns>
    public static HRESULT ForwardNotification<T>(this IEnumerable<DialogControl<T>> handlers, Notification notif)
        => handlers.Select(h => h.HandleNotification(notif)).FirstOrDefault(r => r != default);

    [StackTraceHidden]
    public static InvalidEnumArgumentException InvalidEnumArgumentException<TEnum>(this TEnum value, [CallerArgumentExpression(nameof(value))] string argumentName = "") where TEnum : struct, Enum, IConvertible
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

    public static void SetAsValueOf(this string? value, ref SafeLPWSTR str)
    {
        str.Dispose();
        str = new(value);
    }

    private static nint ToNint<T>(this T val) where T : IConvertible => (nint)val.ToInt64(CultureInfo.InvariantCulture);
}