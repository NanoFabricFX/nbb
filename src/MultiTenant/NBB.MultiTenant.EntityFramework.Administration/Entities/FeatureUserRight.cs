namespace NBB.MultiTenant.EntityFramework.Administration.Entities
{
    public partial class FeatureUserRight<T>
    {
        public T FeatureId { get; set; }
        public T UserRightId { get; set; }

        public virtual Feature<T> Feature { get; set; }
        public virtual UserRight<T> UserRight { get; set; }
    }
}