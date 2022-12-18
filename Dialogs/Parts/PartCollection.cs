using System.Collections;
using static Vanara.PInvoke.ComCtl32;

using Part = Scover.Dialogs.Parts.DialogControl<Scover.Dialogs.Parts.PageUpdateInfo>;

namespace Scover.Dialogs.Parts;

internal sealed class PartCollection : IEnumerable<Part?>
{
    private readonly Dictionary<Type, Part?> _parts = new();

    public event EventHandler<Part>? PartAdded;

    public event EventHandler<Part>? PartRemoved;

    public IEnumerator<Part?> GetEnumerator() => _parts.Values.GetEnumerator();

    public T? GetPart<T>() where T : Part => (T?)_parts.GetValueOrDefault(typeof(T));

    public void SetPart<T>(in TASKDIALOGCONFIG config, T? value) where T : Part
    {
        if (_parts.GetValueOrDefault(typeof(T)) is { } oldPart)
        {
            OnPartRemoved(oldPart);
        }
        _parts[typeof(T)] = value;
        OnPartAdded(value);
        value?.SetIn(config);
    }

    IEnumerator IEnumerable.GetEnumerator() => _parts.Values.GetEnumerator();

    private void OnPartAdded(Part? part)
    {
        if (part is not null)
        {
            PartAdded?.Invoke(this, part);
        }
    }

    private void OnPartRemoved(Part? part)
    {
        if (part is not null)
        {
            PartRemoved?.Invoke(this, part);
        }
    }
}