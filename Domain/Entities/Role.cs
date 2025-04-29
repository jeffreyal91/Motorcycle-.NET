namespace Domain.Entities
{
    public class Role : EntityBase
    {
        public virtual string Name { get; protected set; }
        public virtual string Description { get; set; }
        public virtual ICollection<User> Users { get; protected set; } = new List<User>();

        protected Role() { } // For NHibernate

        public Role(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
        }
    }
}