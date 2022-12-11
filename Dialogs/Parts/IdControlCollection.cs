using System.Collections.ObjectModel;
using System.Diagnostics;
using Vanara.Extensions;
using Vanara.InteropServices;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of objects with IDs.</summary>
public abstract class IdControlCollection<T> : Collection<T>, ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, IStateInitializer, IDisposable where T : notnull
{
    private readonly List<SafeLPWSTR> _nativeButtonTexts = new();
    private SafeNativeArray<TASKDIALOG_BUTTON>? _customButtonArray;
    private T? _defaultItem;
    event EventHandler<Action<PageUpdate>>? IUpdateRequester<PageUpdate>.UpdateRequested { add => UpdateRequested += value; remove => UpdateRequested -= value; }
    private event EventHandler<Action<PageUpdate>>? UpdateRequested;

    /// <summary>Gets or sets the default item of this collection.</summary>
    public T? DefaultItem
    {
        get => _defaultItem;
        set
        {
            if (value is not null && !Contains(value))
            {
                throw new ArgumentException($"Collection doesn not contain value");
            }
            _defaultItem = value;
        }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        foreach (var text in _nativeButtonTexts)
        {
            text.Dispose();
        }
        _nativeButtonTexts.Clear();
        _customButtonArray?.Dispose();
        GC.SuppressFinalize(this);
    }

    void IStateInitializer.InitializeState()
    {
        foreach (var item in Items.OfType<IStateInitializer>())
        {
            item.InitializeState();
        }
    }

    void ILayoutProvider<TASKDIALOGCONFIG>.SetIn(in TASKDIALOGCONFIG container)
    {
        _customButtonArray?.Dispose();

        var customButtons = Items.OfType<INativeProvider<string>>().Select((item, index) =>
        {
            SafeLPWSTR text = new(item.GetNative());
            _nativeButtonTexts.Add(text);
            return new TASKDIALOG_BUTTON
            {
                pszButtonText = text,
                nButtonID = GetId(index)
            };
        }).ToArray();

        _customButtonArray = new(customButtons);

        foreach (var lp in Items.OfType<ILayoutProvider<TASKDIALOGCONFIG>>())
        {
            lp.SetIn(container);
        }

        SetContainerProperties(container, _customButtonArray, (uint)customButtons.Length, DefaultItem is null ? -1 : GetId(IndexOf(DefaultItem)));
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

    private int AssertValidIndex(int index)
    {
        Debug.Assert(index >= 0 && index < Count);
        return index;
    }

    private int GetId(int index) => AssertValidIndex(index) + 1 + CommonButton.MaxId;

    private int GetIndex(int id) => AssertValidIndex(id - 1 - CommonButton.MaxId);

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