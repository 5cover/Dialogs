using System.Collections;
using Vanara.PInvoke;
using Part = Scover.Dialogs.Parts.DialogControl<Scover.Dialogs.Parts.PageUpdateInfo>;

namespace Scover.Dialogs.Parts;

internal sealed class PartCollection : IEnumerable<Part>
{
    private record PartRecord(Part? DefaultValue, Part? Value)
    {
        public Part? Value { get; set; } = Value;
    }

    private readonly Dictionary<Type, PartRecord> _parts = new();

    public event EventHandler<Part>? PartAdded;

    public event EventHandler<Part>? PartRemoved;

    public T? Get<T>() where T : Part
    {
        var p = _parts.GetValueOrDefault(typeof(T));
        return (T?)(p?.Value ?? p?.DefaultValue);
    }

    public IEnumerator<Part> GetEnumerator() => _parts.Values.Select(p => p.Value ?? p.DefaultValue).Where(v => v is not null).GetEnumerator()!; // !: null was filtered out

    public void Set<T>(T? value) where T : Part
    {
        if (_parts.GetValueOrDefault(typeof(T)) is { } oldPart)
        {
            _parts[typeof(T)].Value = value;
            OnPartRemoved(oldPart.Value);
        }
        else
        {
            _parts.Add(typeof(T), new(null, value));
        }
        OnPartAdded(value);
    }

    public void SetDefaultValue<T>(T defaultValue) where T : Part => _parts.Add(typeof(T), new(defaultValue, defaultValue));

    public void SetIn(in ComCtl32.TASKDIALOGCONFIG config)
    {
        foreach (var part in _parts.Values)
        {
            part.Value?.SetIn(config);
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