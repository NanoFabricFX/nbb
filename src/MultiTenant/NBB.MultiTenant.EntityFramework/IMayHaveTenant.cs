using System;

namespace NBB.MultiTenant.EntityFramework
{
    public interface IMayHaveTenant 
    {
        Guid? TenantId { get; set; }
    }

    public interface IMustHaveTenantrewdwsx<T>
    {
        T TenantId { get; set; }
    }

    public interface IMayHaveTenantBun<T>
    {
        T TenantId { get; set; }
    }


    public class MasaTenant<T>
    {
        T TenantId { get; set; }
    }

    public class Mt : IMayHaveTenantBun<Guid?>
    {
        public Guid? TenantId { get; set; }

    }

    public interface ITenantRelated<T>
    {
        T TenantId { get; set; }
    }


    class mimi
    {
        void test()
        {
            var x = new MasaTenant<int?>();
            var x2 = new MasaTenant<int>();
            var x3 = new MasaTenant<Guid>();
            var xsdds = new MasaTenant<Guid?>();
        }
    }

}

// Guid
// int
