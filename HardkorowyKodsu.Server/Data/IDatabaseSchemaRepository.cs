namespace HardkorowyKodsu.Server.Data
{
    public interface IDatabaseSchemaRepository
    {
        Task<List<string>> GetDatabaseObjectsAsync();

        Task<bool> ObjectExistsAsync(string objectName);

        Task<List<SysColumn>> GetColumnsAsync(string objectName);
    }
}