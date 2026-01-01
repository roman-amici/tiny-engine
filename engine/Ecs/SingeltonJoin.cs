namespace TinyEngine.Ecs;


public class SingletonJoin<TSingle,TTable> : IComponentJoin
    where TSingle : class
    where TTable : struct
{
    public SingletonJoin(Singleton<TSingle> single, Table<TTable> table)
    {
        S = single;
        T = table;
    }

    public Singleton<TSingle> S {get;}
    public Table<TTable> T {get;}


    public (TSingle,TTable)? Join
    {
        get
        {
            if (S.Singlet == null)
            {
                return null;
            }

            var t = T.Find(S.EntityId!.Value);

            if (t == null)
            {
                return null;
            }

            return (S.Singlet, t.Value);
        }
    }

        public (EntityId, TSingle,TTable)? JoinWithEntity
    {
        get
        {
            if (S.Singlet == null)
            {
                return null;
            }

            var t = T.Find(S.EntityId!.Value);

            if (t == null)
            {
                return null;
            }

            return (S.EntityId!.Value, S.Singlet, t.Value);
        }
    }

    public void EntityRemoved(EntityId entityId)
    {
        // TODO: Add
    }
}

public class SingletonJoin<TSingle,TTable1, TTable2> : IComponentJoin
    where TSingle : class
    where TTable1 : struct
    where TTable2 : struct
{
    public SingletonJoin(Singleton<TSingle> single, Table<TTable1> table1, Table<TTable2> table2)
    {
        S = single;
        T1 = table1;
        T2 = table2;
    }

    public Singleton<TSingle> S {get;}
    public Table<TTable1> T1 {get;}
    public Table<TTable2> T2 {get;}

    public (TSingle,TTable1,TTable2)? Join
    {
        get
        {
            if (S.EntityId == null)
            {
                return null;
            }

            var t1 = T1.Find(S.EntityId.Value);

            if (t1 == null)
            {
                return null;
            }

            var t2 = T2.Find(S.EntityId.Value);
            if (t2 == null)
            {
                return null;
            }

            return (S.Singlet!, t1.Value, t2.Value);
        }
    }

    public (EntityId, TSingle,TTable1, TTable2)? JoinWithEntity
    {
        get
        {
            var tuple = Join;
            if(tuple == null)
            {
                return null;
            }

            var (s,t1,t2) = tuple.Value;

            return (S.EntityId!.Value,s,t1,t2);
        }
    }

    public void EntityRemoved(EntityId entityId)
    {
        // TODO: Add
    }
}

