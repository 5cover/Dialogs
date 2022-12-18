using System.Collections;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog controls with IDs.</summary>
/// <remarks>This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.</remarks>
public abstract class IdControlCollection<T> : DialogControl<PageUpdateInfo>, ICollection<T>, IDisposable where T : notnull, DialogControl<IdControlUpdateInfo>
{
    private SafeNativeArray<TASKDIALOG_BUTTON>? _nativeArray;

    private protected IdControlCollection(T? defaultItem) => DefaultItem = defaultItem;

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <summary>Gets the default item of this collection.</summary>
    public T? DefaultItem { get; }

    bool ICollection<T>.IsReadOnly => ((ICollection<T>)Items).IsReadOnly;
    private protected virtual TASKDIALOG_FLAGS Flags { get; }
    private protected List<T> Items { get; } = new();

    /// <inheritdoc/>
    public void Add(T item)
    {
        AddItem(item);
        Items.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        foreach (var item in Items)
        {
            RemoveItem(item);
        }
        Items.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item) => Items.Contains(item);

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _nativeArray?.Dispose();
        foreach (var disposable in Items.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        Clear();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        RemoveItem(item);
        return Items.Remove(item);
    }

    /// <inheritdoc/>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    internal virtual T? GetControlFromId(int id)
    {
        var index = GetIndex(id);
        return index >= 0 && index < Items.Count ? Items[index] : null;
    }

    internal override HRESULT HandleNotification(TaskDialogNotification id, nint wParam, nint lParam)
    {
        _ = base.HandleNotification(id, wParam, lParam);
        return Items.ForwardNotification(id, wParam, lParam);
    }

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        foreach (var part in Items)
        {
            part.SetIn(config);
        }

        _nativeArray?.Dispose();
        _nativeArray = new(Items.OfType<ITextControl>().Select((item, index) => new TASKDIALOG_BUTTON
        {
            pszButtonText = (nint)item.NativeText,
            nButtonID = GetId(index)
        }).ToArray());

        config.dwFlags |= Flags;

        int defaultItemIndex = DefaultItem is null ? -1 : Items.IndexOf(DefaultItem);
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
        => OnUpdateRequested(update => e(new(update.Dialog, GetId(Items.IndexOf((T)sender!)))));

    private void RemoveItem(T item) => item.UpdateRequested -= ItemUpdateRequested;
}