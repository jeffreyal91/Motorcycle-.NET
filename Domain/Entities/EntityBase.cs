namespace Domain.Entities
{
    public abstract class EntityBase
    {
        public virtual int Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not EntityBase other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id != 0 && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), Id);
        }

        public static bool operator ==(EntityBase? a, EntityBase? b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase? a, EntityBase? b)
        {
            return !(a == b);
        }
    }
}