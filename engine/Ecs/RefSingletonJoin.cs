namespace TinyEngine.Ecs;

public class RefSingletonJoin<TSingle,TTable> : IComponentJoin
    where TSingle : class
    where TTable : class
{
    public RefSingletonJoin(Singleton<TSingle> single, RefTable<TTable> table)
    {
        S = single;
        T = table;
    }

    public Singleton<TSingle> S {get;}
    public RefTable<TTable> T {get;}

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

            return (S.Singlet, t);
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

            return (S.EntityId!.Value, S.Singlet, t);
        }
    }
}