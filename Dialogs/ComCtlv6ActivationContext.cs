// Adapted from https://github.com/ookii-dialogs/ookii-dialogs-wpf/blob/master/src/Ookii.Dialogs.Wpf/ComCtlv6ActivationContext.cs

// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with the License. You
// may obtain a copy of the License at
//
// https://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS
// IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language
// governing permissions and limitations under the License.

// Changes:
// - Simplified a few selection statements.
// - Simplified a few comments.
// - Adapted the constants.
// - Adapted the code to use Vanara.
// - Use default as NULL pointer constant
// - Merge if statements
// - Divided EnsureActivateContextCreated in 2 methods

using static Vanara.PInvoke.Kernel32;

namespace Scover.Dialogs;

internal sealed class ComCtlV6ActivationContext : IDisposable
{
    private static readonly object contextCreationLock = new();
    private static SafeHACTCTX? activationContext;
    private static bool contextCreationSucceeded;
    private static ACTCTX enableThemingActivationContext;

    private nint _cookie;

    public ComCtlV6ActivationContext(bool enable)
    {
        if (enable && EnsureActivateContextCreated() && !ActivateActCtx(activationContext, out _cookie))
        {
            _cookie = default;
        }
    }

    ~ComCtlV6ActivationContext() => Dispose();

    public void Dispose()
    {
        if (_cookie != default && DeactivateActCtx(default, _cookie))
        {
            // deactivation succeeded
            _cookie = default;
        }
        activationContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    private static string CreateTempManifestFile()
    {
        const string manifestResourceName = $"{nameof(Scover)}.{nameof(Dialogs)}.XPThemes.manifest";
        string manifestTempFilePath;

        using (var manifestStream = typeof(ComCtlV6ActivationContext).Assembly.GetManifestResourceStream(manifestResourceName))
        {
            if (manifestStream is null)
            {
                throw new InvalidOperationException($"Unable to retrieve {manifestResourceName} embedded resource");
            }

            manifestTempFilePath = Path.GetRandomFileName();

            using FileStream tempFileStream = new(manifestTempFilePath, FileMode.CreateNew, System.IO.FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite);
            manifestStream.CopyTo(tempFileStream);
        }

        return manifestTempFilePath;
    }

    private static bool EnsureActivateContextCreated()
    {
        lock (contextCreationLock)
        {
            if (contextCreationSucceeded)
            {
                return true;
            }

            string manifestTempFilePath = CreateTempManifestFile();

            enableThemingActivationContext = new ACTCTX(manifestTempFilePath);

            // Note this will fail gracefully if file specified by manifestFilePath doesn't exist.
            activationContext = CreateActCtx(enableThemingActivationContext);
            contextCreationSucceeded = !activationContext.IsInvalid;

            try
            {
                File.Delete(manifestTempFilePath);
            }
            catch (Exception e) when (e is UnauthorizedAccessException or IOException)
            {
                // It's a temp file, it's fine.
            }

            // If we return false, we'll try again on the next call into EnsureActivateContextCreated(), which is fine.
            return contextCreationSucceeded;
        }
    }
}