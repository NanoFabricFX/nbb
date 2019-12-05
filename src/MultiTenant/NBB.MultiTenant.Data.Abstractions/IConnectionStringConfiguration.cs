namespace NBB.MultiTenant.Data.Abstractions
{
    public interface IConnectionStringConfiguration
    {
        string GetConnectionString();
        void SetConnectionString(string s);
    }
}