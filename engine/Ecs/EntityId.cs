using System.Diagnostics.CodeAnalysis;

namespace TinyEngine.Ecs;

public struct EntityId(ulong id) : IComparable
{
    public ulong Id {get;} = id;

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is EntityId otherId)
        {
            return otherId.Id == Id;
        }

        return false;
    }

    public static bool operator == (EntityId b1, EntityId b2)
    {
        if ((object)b1 == null)
            return (object)b2 == null;

        return b1.Equals(b2);
    }

    public static bool operator != (EntityId b1, EntityId b2)
    {
        return !(b1 == b2);
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public int CompareTo(object? obj)
    {
        if (obj is EntityId e)
        {
            return Id.CompareTo(e.Id);
        }

        return 0;
    }
}