using Microsoft.EntityFrameworkCore;

namespace HardkorowyKodsu.Server.Data
{
    public class DatabaseSchemaRepository : IDatabaseSchemaRepository
    {
        private readonly HardkorowyKodsuDbContext _dbContext;

        private readonly static string[] allowedTypes = { "U", "V" };

        public DatabaseSchemaRepository(HardkorowyKodsuDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetDatabaseObjectsAsync()
        {
            return await _dbContext.SysObjects
                .Where(o => allowedTypes.Contains(o.type))
                .Select(o => o.name).ToListAsync();
        }

        public async Task<bool> ObjectExistsAsync(string objectName)
        {
            return await _dbContext.SysObjects
                .AnyAsync(o => allowedTypes.Contains(o.type) && (o.name == objectName));
        }

        public async Task<List<SysColumn>> GetColumnsAsync(string objectName)
        {
            return await _dbContext.SysColumns
                .Where(c => c.table_name == objectName && c.table_schema == "dbo").ToListAsync();
        }
    }
}
