using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HardkorowyKodsu.Core.DTO;
using HardkorowyKodsu.Server.Services;

namespace HardkorowyKodsu.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseSchemaController : ControllerBase
    {
        private readonly IDatabaseSchemaService _service;

        public DatabaseSchemaController(IDatabaseSchemaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Zwraca listę tabel i widoków.
        /// </summary>
        [HttpGet("objects")]
        public async Task<ActionResult<List<string>>> GetDatabaseObjects()
        {
            var objects = await _service.GetDatabaseObjectsAsync();
            return Ok(objects);
        }

        /// <summary>
        /// Zwraca kolumny dla wybranej tabeli/widoku.
        /// Walidacja parametru: minimalna długość = 1.
        /// </summary>
        [HttpGet("columns/{objectName}")]
        public async Task<ActionResult<List<Column>>> GetColumns([Required, MinLength(1)] string objectName)
        { 
            var columns = await _service.GetColumnsAsync(objectName);

            var result = columns.Select(column => new Column
            {
                Name = column.column_name,
                Type = column.data_type
            }).ToList();

            return Ok(result);
        }
    }
}
