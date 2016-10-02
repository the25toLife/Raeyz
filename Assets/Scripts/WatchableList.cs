using System;
using System.Collections.Generic;

public class WatchableList<T> : List<T>
{
    public event Action CollectionChanged = delegate { };
    public new void Add(T item)
    {
        base.Add(item);
        CollectionChanged();
    }
    public new void Remove(T item)
    {
        base.Remove(item);
        CollectionChanged();
    }
    public new void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        CollectionChanged();
    }
    public new void RemoveRange(int index, int count)
    {
        base.RemoveRange(index, count);
        CollectionChanged();
    }
    public new void Clear()
    {
        base.Clear();
        CollectionChanged();
    }
    public new void Insert(int index, T item)
    {
        base.Insert(index, item);
        CollectionChanged();
    }
    public new void InsertRange(int index, IEnumerable<T> collection)
    {
        base.InsertRange(index, collection);
        CollectionChanged();
    }
    public new void RemoveAll(Predicate<T> match)
    {
        base.RemoveAll(match);
        CollectionChanged();
    }

    public new T this[int index]
    {
        get
        {
            return base[index];
        }
        set
        {
            base[index] = value;
            CollectionChanged();
        }
    }
}