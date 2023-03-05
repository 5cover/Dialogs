using System.Collections;

using Vanara.Extensions;
using Vanara.InteropServices;

using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs;

/// <summary>A collection of dialog controls with IDs.</summary>
/// <remarks>
/// This class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its
/// items.
/// </remarks>
public abstract class IdControlCollection<T> : DialogControl<PageUpdateInfo>, ICollection<T>, IDisposable where T : notnull, DialogControl<IdControlUpdateInfo>
{
    private readonly BidirectionalDictionary<T, int> _ids;
    private readonly Dictionary<TASKDIALOGCONFIG, SafeNativeArray<TASKDIALOG_BUTTON>> _nativeArrays = new();
    private int _id;

    /// <param name="items">The initial items.</param>
    /// <param name="defaultItem">The default item.</param>
    protected IdControlCollection(IList<T>? items, T? defaultItem)
        => (_ids, _id, DefaultItem) = (items is null ? new() : new(items.ToDictionary(item => item, MakeNewId)), StartId, defaultItem);

    /// <inheritdoc/>
    public int Count => _ids.Count;

    /// <summary>Gets or sets the default item of this collection.</summary>
    /// <remarks>
    /// Setting the value to an item that is not in the collection will not throw an exception.
    /// </remarks>
    public T? DefaultItem { get; set; }

    bool ICollection<T>.IsReadOnly => false;

    /// <summary>The starting ID.</summary>
    /// <remarks>
    /// IDs are 1-based (0 means none). This value must be initialized appropriately to avoid collisions
    /// between the IDs of the items that implement <see cref="IHasId"/> and those that don't.
    /// </remarks>
    protected virtual int StartId => 1;

    /// <summary>
    /// Flags to add to <see cref="TASKDIALOGCONFIG.dwFlags"/> in <see cref="SetIn(in TASKDIALOGCONFIG)"/>.
    /// </summary>
    protected virtual TASKDIALOG_FLAGS Flags { get; }

    /// <inheritdoc/>
    public void Add(T item)
    {
        AddItem(item);
        _ids.Add(item, MakeNewId(item));
    }

    /// <inheritdoc/>
    public void Clear()
    {
        foreach (var item in _ids.Keys)
        {
            RemoveItem(item);
        }
        _ids.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item) => _ids.ContainsKey(item);

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        foreach (var nativeArray in _nativeArrays.Values)
        {
            nativeArray.Dispose();
        }
        foreach (var disposable in _ids.Keys.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        Clear();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _ids.Keys.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        RemoveItem(item);
        return _ids.Remove(item);
    }

    /// <inheritdoc/>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _ids.Keys.CopyTo(array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() => _ids.Keys.GetEnumerator();

    internal T? GetItem(int id) => _ids.Inverse.GetValueOrDefault(id);

    internal override void SetIn(in TASKDIALOGCONFIG config)
    {
        foreach (var part in _ids.Keys)
        {
            part.SetIn(config);
        }

        SafeNativeArray<TASKDIALOG_BUTTON> nativeArray = new(_ids.Keys.Where(v => v is ITextControl).Select(item => new TASKDIALOG_BUTTON
        {
            pszButtonText = (nint)((ITextControl)item).NativeText,
            nButtonID = _ids[item]
        }).ToArray());

        _nativeArrays.GetValueOrDefault(config)?.Dispose();
        _nativeArrays[config] = nativeArray;

        config.dwFlags |= Flags;
        SetConfigProperties(config, nativeArray, (uint)nativeArray.Count, DefaultItem is null ? 0 : _ids[DefaultItem]);
    }

    /// <summary>
    /// Sets the appropriate fields and properties in <paramref name="config"/> for the given arguments.
    /// </summary>
    /// <param name="config">The object to configure.</param>
    /// <param name="nativeButtonArrayHandle">The handle to the array containing the native buttons.</param>
    /// <param name="nativeButtonArrayCount">The count of native buttons.</param>
    /// <param name="defaultItemId">The ID of the default item.</param>
    protected abstract void SetConfigProperties(in TASKDIALOGCONFIG config, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId);

    private int MakeNewId(T item) => item is IHasId hasId ? hasId.Id : _id++;

    private void AddItem(T item)
    {
        if (Contains(item))
        {
            throw new InvalidOperationException($"The item '{item}' is already in the collection.");
        }
        item.UpdateRequested += ItemUpdateRequested;
    }

    private void ItemUpdateRequested(object? sender, Action<IdControlUpdateInfo> e)
        => RequestUpdate(info => e(new(info.Dialog, _ids[(T)sender.AssertNotNull()])));

    private void RemoveItem(T item) => item.UpdateRequested -= ItemUpdateRequested;
}