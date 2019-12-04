namespace NBB.MultiTenant.EntityFramework.Administration.Entities
{
    public partial class RoleUserRight<T>
    {
        public T RoleId { get; set; }
        public T UserRightId { get; set; }

        public virtual Role<T> Role { get; set; }
        public virtual UserRight<T> UserRight { get; set; }
    }
}