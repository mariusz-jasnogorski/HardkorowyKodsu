using Moq;
using HardkorowyKodsu.Server.Services;
using HardkorowyKodsu.Server.Data;

namespace HardkorowyKodsu.Tests.Unit
{
    public class DatabaseSchemaServiceTests
    {
        private readonly Mock<IDatabaseSchemaRepository> _mockRepo;
        private readonly IDatabaseSchemaService _service;

        public DatabaseSchemaServiceTests()
        {
            _mockRepo = new Mock<IDatabaseSchemaRepository>();

            _service = new DatabaseSchemaService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetColumnsAsync_ExistingObject_ReturnsColumns()
        {
            // Arrange
            string objectName = "TestTable";
            var columnsStub = new List<SysColumn>
            {
                new SysColumn { ordinal_position = 1, column_name = "Id", data_type = "int", table_name = objectName, table_schema = "dbo" },
                new SysColumn { ordinal_position = 2, column_name = "Name", data_type = "nvarchar", table_name = objectName, table_schema = "dbo" }
            };

            _mockRepo.Setup(r => r.ObjectExistsAsync(objectName)).ReturnsAsync(true);

            _mockRepo.Setup(r => r.GetColumnsAsync(objectName)).ReturnsAsync(columnsStub);

            // Act
            var result = await _service.GetColumnsAsync(objectName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.column_name == "Id" && c.data_type == "int");
        }

        [Fact]
        public async Task GetColumnsAsync_NonExistingObject_ThrowsArgumentException()
        {
            // Arrange
            string objectName = "UnknownTable";

            _mockRepo.Setup(r => r.ObjectExistsAsync(objectName))
                     .ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return _service.GetColumnsAsync(objectName);
            });

            Assert.Contains(objectName, ex.Message);
        }
    }
}
