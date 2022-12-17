using System.Collections.ObjectModel;
using Vanara.Extensions;
using Vanara.InteropServices;
using static Vanara.PInvoke.ComCtl32;

namespace Scover.Dialogs.Parts;

/// <summary>A collection of objects with IDs. This class implements <see cref="IDisposable"/>.</summary>
public abstract class IdControlCollection<T> : Collection<T>, ILayoutProvider<TASKDIALOGCONFIG>, IUpdateRequester<PageUpdate>, IStateInitializer, IDisposable where T : notnull
{
    private T? _defaultItem;
    private GenericSafeHandle? _native;
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
                throw new ArgumentException($"Collection does not contain value.");
            }
            _defaultItem = value;
        }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _native?.Dispose();
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

        List<SafeLPWSTR> nativeButtonTexts = new();
        SafeNativeArray<TASKDIALOG_BUTTON>? customButtonArray;

        customButtonArray = new(Items.OfType<INativeProvider<string>>().Select((item, index) =>
        {
            SafeLPWSTR text = new(item.GetNative());
            nativeButtonTexts.Add(text);
            return new TASKDIALOG_BUTTON
            {
                pszButtonText = text,
                nButtonID = GetId(index)
            };
        }).ToArray());

        _native?.Dispose();
        _native = new(customButtonArray, _ =>
        {
            customButtonArray.Dispose();
            foreach (var text in nativeButtonTexts)
            {
                text.Dispose();
            }
            return true;
        });

        SetContainerProperties(container, customButtonArray, (uint)customButtonArray.Count, DefaultItem is null ? 0 : GetId(IndexOf(DefaultItem)));
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