using Microsoft.AspNetCore.Mvc;
using HardkorowyKodsu.Server.Controllers;
using HardkorowyKodsu.Server.Data;
using HardkorowyKodsu.Server.Services;
using Microsoft.EntityFrameworkCore;
using HardkorowyKodsu.Core.DTO;

namespace HardkorowyKodsu.Tests.Integration
{
    public class DatabaseSchemaControllerTests
    {
        private readonly DatabaseSchemaController _controller;
        private readonly HardkorowyKodsuDbContext _dbContext;

        public DatabaseSchemaControllerTests()
        {
            // Konfiguracja kontekstu EF Core InMemory
            var options = new DbContextOptionsBuilder<HardkorowyKodsuDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _dbContext = new HardkorowyKodsuDbContext(options);

            // Dodanie przykładowych danych
            _dbContext.SysObjects.RemoveRange(_dbContext.SysObjects);
            _dbContext.SysObjects.Add(new SysObject { Id = 1, name = "TestTable", type = "U" });

            _dbContext.SysColumns.RemoveRange(_dbContext.SysColumns);
            _dbContext.SysColumns.Add(new SysColumn { ordinal_position = 1, column_name = "Id", data_type = "int", table_name = "TestTable" });
            _dbContext.SysColumns.Add(new SysColumn { ordinal_position = 2, column_name = "Name", data_type = "varchar", table_name = "TestTable" });

            _dbContext.SaveChanges();

            var repo = new DatabaseSchemaRepository(_dbContext);
            var service = new DatabaseSchemaService(repo);
            _controller = new DatabaseSchemaController(service);
        }

        [Fact]
        public async Task GetDatabaseObjects_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetDatabaseObjects();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var objList = Assert.IsType<List<string>>(okResult.Value);

            Assert.Single(objList);
            Assert.Contains("TestTable", objList);
        }

        [Fact]
        public async Task GetColumns_ExistingObject_ReturnsOkAndColumns()
        {
            // Arrange
            var objectName = "TestTable";

            // Act
            var result = await _controller.GetColumns(objectName);

            // Assert
            var okResult = Assert.IsType<ActionResult<List<Column>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var columns = Assert.IsType<List<Column>>(objectResult.Value);

            Assert.Equal(2, columns.Count);         
            Assert.Contains(columns, c => c.Name == "Id" && c.Type == "int");
            Assert.Contains(columns, c => c.Name == "Name" && c.Type == "varchar");
        }

        [Fact]
        public async Task GetColumns_NonExistingObject_ShouldThrowException()
        {
            // Arrange
            var objectName = "UnknownObject";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _controller.GetColumns(objectName));
        }

        [Fact]
        public async Task GetDatabaseObjects_ReturnsOk_WithList()
        {
            // Act
            var result = await _controller.GetDatabaseObjects();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var objects = Assert.IsType<List<string>>(okResult.Value);
            Assert.Contains("TestTable", objects);
        }

        [Fact]
        public async Task GetColumns_ExistingObject_ReturnsOk_WithColumns()
        {
            // Act
            var result = await _controller.GetColumns("TestTable");

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Column>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var columns = Assert.IsType<List<Column>>(okResult.Value);
            Assert.Equal(2, columns.Count);
        }
    }
}
