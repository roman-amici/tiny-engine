using System.Collections;

namespace TinyEngine.Ecs;

public class TableJoin<T, U> : IEnumerable<(T, U)>, IComponentJoin
where T : struct
where U : struct

{
    public TableJoin(Table<T> t1, Table<U> t2)
    {
        T1 = t1;
        T2 = t2;
    }

    public Table<T> T1 {get;}
    public Table<U> T2 {get;}


    public (T,U) this[int indexT, int indexU]
    {
        get
        {
            return (T1[indexT],T2[indexU]);
        }
    }

    public (T,U) this[EntityId entityId]
    {
        get
        {
            var t = T1[entityId];
            var u = T2[entityId];

            return (t,u);
        }
    }

    public (T,U)? Find(EntityId entityId)
    {
        var t = T1.Find(entityId);
        if (t == null)
        {
            return null;
        }

        var u = T2.Find(entityId);
        if (u == null)
        {
            return null;
        }

        return (t.Value,u.Value);
    }

    public IEnumerator<(T, U)> GetEnumerator()
    {
        foreach (var (i,j) in Indices())
        {
            yield return (T1[i],T2[j]);
        }
    }

    public IEnumerable<(Component<T>,Component<U>)> Components()
    {
        foreach (var (i,j) in Indices())
        {
            yield return (T1.GetComponent(i),T2.GetComponent(j));
        }
    }

    public IEnumerable<(int,int)> Indices()
    {
        // Optimization since we know that tables are in sorted order
        var lastJ = 0;
        for (var i = 0; i < T1.Count; i++)
        {
            for (var j = lastJ; j < T2.Count; j++)
            {
                if (T1.GetComponent(i).EntityId == T2.GetComponent(j).EntityId)
                {
                    yield return (i,j);

                    lastJ = j+1;
                    break;
                }
            }

            if (lastJ >= T2.Count)
            {
                yield break;
            }
        }
    }

    public IEnumerable<(EntityId, T, U)> WithEntityId()
    {
        foreach( var (c1,c2) in Components())
        {
            yield return (c1.EntityId, c1.Value, c2.Value);
        }
    }

    public (T,U)? FindWhere(Func<(T,U),bool> predicate)
    {
        foreach (var (i,j) in Indices())
        {
            if (predicate((T1[i],T2[j])))
            {
                return (T1[i],T2[j]);
            }
        }

        return null;
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void IComponentJoin.EntityRemoved(EntityId _)
    {
        // TODO: Add
    }
}

public class TableJoin<T, U, V> : IEnumerable<(T, U, V)>, IComponentJoin
where T : struct
where U : struct
where V : struct

{
    public TableJoin(Table<T> t1, Table<U> t2, Table<V> t3)
    {
        T1 = t1;
        T2 = t2;
        T3 = t3;
    }

    public Table<T> T1 {get;}
    public Table<U> T2 {get;}
    public Table<V> T3 {get;}


    public (T,U,V) this[int indexT, int indexU, int indexV]
    {
        get
        {
            return (T1[indexT],T2[indexU],T3[indexV]);
        }
    }

    public (T,U,V) this[EntityId entityId]
    {
        get
        {
            var t = T1[entityId];
            var u = T2[entityId];
            var v = T3[entityId];

            return (t,u,v);
        }
    }

    public (T,U,V)? Find(EntityId entityId)
    {
        var t = T1.Find(entityId);
        if (t == null)
        {
            return null;
        }

        var u = T2.Find(entityId);
        if (u == null)
        {
            return null;
        }

        var v = T3.Find(entityId);
        if (v == null)
        {
            return null;
        }

        return (t.Value,u.Value, v.Value);
    }

    public IEnumerator<(T, U, V)> GetEnumerator()
    {
        foreach (var (i,j,k) in Indices())
        {
            yield return (T1[i],T2[j],T3[k]);
        }
    }

    public IEnumerable<(Component<T>,Component<U>,Component<V>)> Components()
    {
        foreach (var (i,j,k) in Indices())
        {
            yield return (T1.GetComponent(i),T2.GetComponent(j),T3.GetComponent(k));
        }
    }

    public IEnumerable<(int,int,int)> Indices()
    {
        // Optimization since we know that tables are in sorted order
        var lastJ = 0;
        var lastK = 0;
        for (var i = 0; i < T1.Count; i++)
        {
            var entityId = T1.GetComponent(i).EntityId;
            for (var j = lastJ; j < T2.Count; j++)
            {
                if (entityId != T2.GetComponent(j).EntityId)
                {
                    continue;
                }

                for (var k = lastK; k < T3.Count; k++)
                {
                    if (entityId == T3.GetComponent(k).EntityId)
                    {
                        yield return (i,j,k);
                        lastJ = j;
                        lastK = k;
                        j = T2.Count;
                        k = T3.Count;
                    }
                }
            }
        }
    }

    public IEnumerable<(EntityId, T, U, V)> WithEntityId()
    {
        foreach( var (c1,c2,c3) in Components())
        {
            yield return (c1.EntityId, c1.Value, c2.Value,c3.Value);
        }
    }

    public (T,U,V)? FindWhere(Func<(T,U,V),bool> predicate)
    {
        foreach (var (i,j,k) in Indices())
        {
            if (predicate((T1[i],T2[j],T3[k])))
            {
                return (T1[i],T2[j],T3[k]);
            }
        }

        return null;
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void IComponentJoin.EntityRemoved(EntityId _)
    {
       // TODO: Add
    }
}