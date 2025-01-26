using HardkorowyKodsu.Server.Data;

namespace HardkorowyKodsu.Server.Services
{
    public class DatabaseSchemaService : IDatabaseSchemaService
    {
        private readonly IDatabaseSchemaRepository _repository;

        public DatabaseSchemaService(IDatabaseSchemaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> GetDatabaseObjectsAsync()
        {
            // Tu można dodać logikę biznesową, np. cachowanie
            return await _repository.GetDatabaseObjectsAsync();
        }

        public async Task<List<SysColumn>> GetColumnsAsync(string objectName)
        {
            bool exists = await _repository.ObjectExistsAsync(objectName);

            if (!exists)
            {
                // Wyjątek, który przechwyci global exception handler
                throw new ArgumentException($"Obiekt '{objectName}' nie istnieje w bazie danych.");
            }

            return await _repository.GetColumnsAsync(objectName);
        }
    }
}
