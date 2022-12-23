using System.Collections;
using Vanara.Extensions;
using Vanara.InteropServices;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog controls with IDs.</summary>
/// <remarks>This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.</remarks>
public abstract class IdControlCollection<T> : DialogControl<PageUpdateInfo>, ICollection<T>, IDisposable where T : notnull, DialogControl<IdControlUpdateInfo>
{
    private readonly List<T> _items = new();
    private SafeNativeArray<TASKDIALOG_BUTTON>? _nativeArray;

    private protected IdControlCollection(T? defaultItem) => DefaultItem = defaultItem;

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <summary>Gets the default item of this collection.</summary>
    public T? DefaultItem { get; }

    bool ICollection<T>.IsReadOnly => ((ICollection<T>)_items).IsReadOnly;
    private protected virtual TASKDIALOG_FLAGS Flags { get; }

    /// <inheritdoc/>
    public void Add(T item)
    {
        AddItem(item);
        _items.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        foreach (var item in _items)
        {
            RemoveItem(item);
        }
        _items.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item) => _items.Contains(item);

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _nativeArray?.Dispose();
        foreach (var disposable in _items.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        Clear();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        RemoveItem(item);
        return _items.Remove(item);
    }

    /// <inheritdoc/>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    internal virtual T? GetControlFromId(int id)
    {
        var index = GetIndex(id);
        return index >= 0 && index < _items.Count ? _items[index] : null;
    }

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        foreach (var part in _items)
        {
            part.SetIn(config);
        }

        _nativeArray?.Dispose();
        _nativeArray = new(_items.OfType<ITextControl>().Select((item, index) => new TASKDIALOG_BUTTON
        {
            pszButtonText = (nint)item.NativeText,
            nButtonID = GetId(index)
        }).ToArray());

        config.dwFlags |= Flags;

        int defaultItemIndex = DefaultItem is null ? -1 : _items.IndexOf(DefaultItem);
        SetConfigProperties(config, _nativeArray, (uint)_nativeArray.Count, defaultItemIndex == -1 ? 0 : GetId(defaultItemIndex));
    }

    private protected virtual int GetId(int index) => index + 1;

    private protected virtual int GetIndex(int id) => id - 1;

    private protected abstract void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId);

    private void AddItem(T item)
    {
        if (Contains(item))
        {
            throw new InvalidOperationException($"The item '{item}' is already in the collection.");
        }
        item.UpdateRequested += ItemUpdateRequested;
    }

    private void ItemUpdateRequested(object? sender, Action<IdControlUpdateInfo> e)
        => RequestUpdate(info => e(new(info.Dialog, GetId(_items.IndexOf((T)sender.AssertNotNull())))));

    private void RemoveItem(T item) => item.UpdateRequested -= ItemUpdateRequested;
}