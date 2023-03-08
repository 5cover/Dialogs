// Adapted from
// https://github.com/ookii-dialogs/ookii-dialogs-wpf/blob/master/src/Ookii.Dialogs.Wpf/ComCtlv6ActivationContext.cs

// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
// https://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// Changes:
// - Adapted the code to use Vanara.
// - Use default as NULL pointer constant
// - Simplified creation logic to not use a temp file.
// - Used Lazy<T> to simplify logic
// - Adapted to Vanara semantics (safe handles and Win32Error)

using System.Reflection;

using Vanara.InteropServices;
using Vanara.PInvoke;

using static Vanara.PInvoke.Kernel32;

namespace Scover.Dialogs;

internal sealed class ComCtlV6ActivationContext : IDisposable
{
    private static readonly Lazy<SafeHACTCTX> activationContext = new(CreateActivationContext);
    private static readonly object contextCreationLock = new();
    private readonly GenericSafeHandle? _cookie;

    public ComCtlV6ActivationContext()
    {
        _ = Win32Error.ThrowLastErrorIfFalse(ActivateActCtx(activationContext.Value, out nint cookie), "ComCtl32 V6 activation context activation failed.");
        _cookie = new(cookie, static ptr => Win32Error.ThrowLastErrorIfFalse(DeactivateActCtx(default, ptr), "ComCtl32 V6 activation context deactivation failed."));
    }

    public void Dispose()
    {
        _cookie?.Dispose();
        activationContext.Value?.Dispose();
    }

    private static SafeHACTCTX CreateActivationContext()
    {
        lock (contextCreationLock)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var xpThemes = assembly.GetManifestResourceStream($"{nameof(Scover)}.{nameof(Scover.Dialogs)}.XPThemes.manifest").AssertNotNull();
            var manifestFilePath = Path.Join(Path.GetDirectoryName(assembly.Location), CreateTempFile(xpThemes));
            return Win32Error.ThrowLastErrorIfInvalid(CreateActCtx(new ACTCTX(manifestFilePath)));
        }
    }

    private static string CreateTempFile(Stream contents)
    {
        using FileStream file = new(Path.GetTempFileName(), FileMode.Open);
        contents.CopyTo(file);
        return file.Name;
    }
}