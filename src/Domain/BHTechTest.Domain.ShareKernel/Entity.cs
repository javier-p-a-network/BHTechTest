namespace BHTechTest.Domain.ShareKernel
{
    namespace BHTechTest.Domain.SharedKernel
    {
        public abstract class Entity
        {
            public int Id { get; protected set; }

            public override bool Equals(object? obj)
            {
                if (obj is not Entity other)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                if (Id.Equals(default) || other.Id.Equals(default))
                    return false;
                return Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return Id.Equals(default) ? base.GetHashCode() : Id.GetHashCode();
            }

            public static bool operator ==(Entity? a, Entity? b)
            {
                if (a is null && b is null)
                    return true;
                if (a is null || b is null)
                    return false;
                return a.Equals(b);
            }
            public static bool operator !=(Entity? a, Entity? b)
            {
                return !(a == b);
            }
        }
    }
}
