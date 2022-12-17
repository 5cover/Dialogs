using System.Collections.ObjectModel;
using Vanara.Extensions;
using Vanara.InteropServices;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of dialog controls with IDs.</summary>
/// <remarks>
/// This collection class implements <see cref="IDisposable"/> and calls <see cref="IDisposable.Dispose"/> on its items.
/// </remarks>
public abstract class IdControlCollection<T> : Collection<T>, ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, IStateInitializer, IDisposable where T : notnull
{
    private SafeNativeArray<TASKDIALOG_BUTTON>? _nativeArray;

    private protected IdControlCollection(T? defaultItem) => DefaultItem = defaultItem;

    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets the default item of this collection.</summary>
    public T? DefaultItem { get; }

    private protected virtual TASKDIALOG_FLAGS Flags { get; }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _nativeArray?.Dispose();
        foreach (var disposable in Items.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    void IStateInitializer.InitializeState()
    {
        foreach (var stateInitializer in Items.OfType<IStateInitializer>())
        {
            stateInitializer.InitializeState();
        }
    }

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container)
    {
        foreach (var layoutProvider in Items.OfType<ILayoutProvider<TASKDIALOGCONFIG>>())
        {
            layoutProvider.SetIn(container);
        }

        _nativeArray?.Dispose();
        _nativeArray = new(Items.OfType<INativeProvider<StrPtrUni>>().Select((item, index) => new TASKDIALOG_BUTTON
        {
            pszButtonText = (nint)item.GetNative(),
            nButtonID = GetId(index)
        }).ToArray());

        container.dwFlags |= Flags;

        int defaultItemIndex = DefaultItem is null ? -1 : IndexOf(DefaultItem);
        SetContainerProperties(container, _nativeArray, (uint)_nativeArray.Count, defaultItemIndex == -1 ? 0 : GetId(defaultItemIndex));
    }

    internal virtual T? GetControlFromId(int id) => id == 0 ? default : Items[GetIndex(id)];

    /// <inheritdoc/>
    protected override void ClearItems()
    {
        foreach (var item in Items)
        {
            RemoveItem(item);
        }
        base.ClearItems();
    }

    private protected virtual int GetId(int index) => index + 1;

    private protected virtual int GetIndex(int id) => id - 1;

    /// <inheritdoc/>
    protected override void InsertItem(int index, T item)
    {
        AddItem(item);
        base.InsertItem(index, item);
    }

    /// <inheritdoc/>
    protected override void RemoveItem(int index)
    {
        RemoveItem(Items[index]);
        base.RemoveItem(index);
    }

    private protected abstract void SetContainerProperties(in TASKDIALOGCONFIG container, nint nativeButtonArrayHandle, uint nativeButtonArrayCount, int defaultItemId);

    /// <inheritdoc/>
    protected override void SetItem(int index, T item)
    {
        RemoveItem(Items[index]);
        AddItem(item);
        base.SetItem(index, item);
    }

    private void AddItem(T item)
    {
        if (Contains(item))
        {
            throw new InvalidOperationException($"The item '{item}' is already in the collection.");
        }
        if (item is IUpdateRequester<IdControlUpdate> ur)
        {
            ur.UpdateRequested += ItemUpdateRequested;
        }
    }

    private void ItemUpdateRequested(object? sender, Action<IdControlUpdate> e)
        => UpdateRequested?.Invoke(this, update => e(new(update.Dialog, GetId(IndexOf((T)sender!)))));

    private void RemoveItem(T item)
    {
        if (item is IUpdateRequester<IdControlUpdate> ur)
        {
            ur.UpdateRequested -= ItemUpdateRequested;
        }
    }
}