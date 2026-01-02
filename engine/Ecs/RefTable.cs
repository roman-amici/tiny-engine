using System.Collections;

namespace TinyEngine.Ecs;

public class RefTable<T> : IEnumerable<T>, IComponentContainer where T : class
{
    private List<EntityId> ids = new();
    private List<T> data = new();

    public int Epoch {get; private set;}

    public int Count => data.Count;

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return data[i];
        }
    }

    public IEnumerable<EntityId> EntityIds()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return ids[i];
        }
    }

    public IEnumerable<(EntityId,T)> WithEntityId()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return (ids[i],data[i]);
        }
    }

    public IEnumerable<int> Indices()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return i;
        }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(EntityId entityId, T value)
    {
        if (value == null)
        {
            throw new ArgumentException(nameof(value), "Value cannot be null");
        }

        Epoch++;

        for (var i = 0; i < data.Count; i++)
        {
            if(ids[i] == entityId)
            {
                throw new InvalidOperationException($"Entity {entityId} already added.");
            }

            if (entityId.Id > ids[i].Id)
            {
                ids.Insert(i, entityId);
                data.Insert(i, value);
                return;
            }
        }

        ids.Add(entityId);
        data.Add(value);
    }

    public void Update(int i, T value)
    {
        data[i] = value;
    }

    public int Update(EntityId entityId, T value)
    {
        for(var i = 0; i < ids.Count; i++)
        {
            if (ids[i] == entityId)
            {
                Update(i,value);

                return i;
            }
        }

        return -1;
    }

    public EntityId GetEntityId(int i)
    {
        return ids[i];
    }

    public T this[int i]
    {
        get => data[i];
    }

    public T this[EntityId entityId]
    {
        get
        {
            var e = Find(entityId);
            if (e == null)
            {
                throw new IndexOutOfRangeException($"No entity {entityId} in table");
            }

            return e;
        }
    }

    public T? Find(EntityId entityId)
    {
        var i = ids.BinarySearch(entityId);
        if (i >= 0)
        {
            return data[i];
        }
        else
        {
            return null;
        }
    }

    public bool Remove(EntityId entityId)
    {
        for (var i = 0; i < data.Count; i++)
        {
            if (ids[i] == entityId)
            {
                data.RemoveAt(i);

                Epoch++;
                return true;
            }
        }

        return false;
    }

    public T? FindWhere(Func<T, bool> predicate)
    {
        foreach(var value in data)
        {
            if (predicate(value))
            {
                return value;
            }
        }

        return null;
    }

     void IComponentContainer.RemoveEntity(EntityId entityId)
    {
        Remove(entityId);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}