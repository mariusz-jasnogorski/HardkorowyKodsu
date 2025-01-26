namespace HardkorowyKodsu.Server.Services
{
    public interface IDatabaseSchemaService
    {
        Task<List<string>> GetDatabaseObjectsAsync();

        Task<List<SysColumn>> GetColumnsAsync(string objectName);
    }
}
