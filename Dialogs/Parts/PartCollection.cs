namespace Scover.Dialogs.Parts;

internal sealed class PartCollection
{
    private readonly Dictionary<Type, object?> _parts = new();

    public event EventHandler<object?>? PartAdded;

    public event EventHandler<object?>? PartRemoved;

    public T? GetPart<T>() => (T?)_parts.GetValueOrDefault(typeof(T));

    public IEnumerable<T> GetParts<T>() => _parts.Values.OfType<T>();

    public void SetPart<T, TContainer>(in TContainer container, T? value)
    {
        if (_parts.GetValueOrDefault(typeof(T)) is { } oldPart)
        {
            OnPartRemoved(oldPart);
        }

        _parts[typeof(T)] = value;
        if (value is ILayoutProvider<TContainer> lp)
        {
            lp.SetIn(container);
        }
        OnPartAdded(value);
    }

    private void OnPartAdded(object? part) => PartAdded?.Invoke(this, part);

    private void OnPartRemoved(object? part) => PartRemoved?.Invoke(this, part);
}