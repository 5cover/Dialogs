using System.Collections;
using System.Runtime.CompilerServices;

using Vanara.PInvoke;

using Part = Scover.Dialogs.DialogControl<Scover.Dialogs.PageUpdateInfo>;

namespace Scover.Dialogs;

internal sealed class PartCollection : IEnumerable<Part>
{
    private sealed record PartRecord(Part? DefaultValue, Part? Value)
    {
        public Part? Value { get; set; } = Value;
    }

    private readonly Dictionary<string, PartRecord> _parts = new();

    public event TypeEventHandler<PartCollection, Part>? PartAdded;

    public event TypeEventHandler<PartCollection, Part>? PartRemoved;

    public Part? Get([CallerMemberName] string name = "")
    {
        var part = _parts.GetValueOrDefault(name);
        return part is null ? null : (part.Value ?? part.DefaultValue);
    }

    public IEnumerator<Part> GetEnumerator() => _parts.Values.Select(p => p.Value ?? p.DefaultValue).Where(v => v is not null).GetEnumerator()!; // !: null has been filtered out

    public void Set(Part? value, Part? defaultValue = null, [CallerMemberName] string name = "")
    {
        if (_parts.GetValueOrDefault(name) is { } oldPart)
        {
            _parts[name].Value = value;
            OnPartRemoved(oldPart.Value);
        }
        else
        {
            _parts.Add(name, new(defaultValue, value));
        }
        OnPartAdded(value);
    }

    public void SetDefault(Part defaultValue, string name) => Set(defaultValue, defaultValue, name);

    public void SetPartsIn(in ComCtl32.TASKDIALOGCONFIG config)
    {
        foreach (string partName in _parts.Keys)
        {
            Get(partName)?.SetIn(config);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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