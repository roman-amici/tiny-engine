using System.Collections;

namespace TinyEngine.Ecs;

public class RefTableJoin<T, U> : IEnumerable<(T, U)>, IComponentJoin
where T : class
where U : class
{
    public RefTableJoin(RefTable<T> t1, RefTable<U> t2)
    {
        T1 = t1;
        T2 = t2;
    }

    public RefTable<T> T1 {get;}
    public RefTable<U> T2 {get;}

    private (int,int) LastEpoch {get; set;} = (-1,-1);
    private bool CacheValid
    {
        get => LastEpoch == (T1.Epoch,T2.Epoch);
    }

    private List<(int,int)> indexCache = new();


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

        return (t,u);
    }

    public IEnumerator<(T, U)> GetEnumerator()
    {
        foreach (var (i,j) in Indices())
        {
            yield return (T1[i],T2[j]);
        }
    }

    public IEnumerable<(int,int)> Indices()
    {
        if (!CacheValid)
        {
            BuildIndexCache();
        }

        return indexCache;
    }

    private void BuildIndexCache()
    {
        indexCache.Clear();

        // Optimization since we know that tables are in sorted order
        var lastJ = 0;
        for (var i = 0; i < T1.Count; i++)
        {
            for (var j = lastJ; j < T2.Count; j++)
            {
                if (T1.GetEntityId(i) == T2.GetEntityId(j))
                {
                    indexCache.Add((i,j));

                    lastJ = j+1;
                    break;
                }
            }

            if (lastJ >= T2.Count)
            {
                break;
            }
        }

        LastEpoch = (T1.Epoch,T2.Epoch);
    }

    public IEnumerable<(EntityId, T, U)> WithEntityId()
    {
        foreach( var (i,j) in Indices())
        {
            yield return (T1.GetEntityId(i), T1[i], T2[j]);
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
}