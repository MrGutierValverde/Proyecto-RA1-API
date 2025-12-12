using APIrpoyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIrpoyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MonsterDAOsqlController : ControllerBase
    {
        private readonly MonsterDAOsql _monsterDAOsql;

        // Constructor que recibe la instancia de MonsterDAOsql (Inyección de dependencias)
        public MonsterDAOsqlController(MonsterDAOsql monsterDAOsql)
        {
            _monsterDAOsql = monsterDAOsql;
        }

        // Insertar un nuevo monstruo
        [HttpPost("Insert")]
        [Authorize(Roles = "Admin")]
        public IActionResult Insert([FromBody] MonsterDTO monster)
        {
            if (monster == null)
            {
                return BadRequest("Monster data is required.");
            }

            _monsterDAOsql.Insert(monster);
            return Ok(true); // Petición exitosa
        }
        // Insertar varios monstruos
        [HttpPost("InsertMany")]
        [Authorize(Roles = "Admin")]
        public IActionResult InsertMany([FromBody] List<MonsterDTO> monsters)
        {
            if (monsters.Count <= 0)
            {
                return BadRequest("Monster data is required.");
            }

            _monsterDAOsql.InsertMany(monsters);
            return Ok(true); // Petición exitosa
        }
        // Actualizar un monstruo existente
        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody] MonsterDTO monster)
        {
            if (monster == null)
            {
                return BadRequest("Monster data is required.");
            }

            _monsterDAOsql.Update(monster);
            return Ok(true); // Petición exitosa
        }

        // Eliminar un monstruo por ID
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _monsterDAOsql.Delete(id);
            return Ok(true); // Petición exitosa
        }

        // Obtener un monstruo por ID
        [HttpGet("GetById/{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            MonsterDTO monster = _monsterDAOsql.GetById(id);
            if (monster != null)
            {
                return Ok(monster); // Devolver objeto MonsterDTO
            }

            return NotFound("Monster not found.");
        }

        // Obtener todos los monstruos
        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAll()
        {
            List<MonsterDTO> monsters = _monsterDAOsql.GetAll();
            if (monsters != null && monsters.Count > 0)
            {
                return Ok(monsters); // Devolver lista de MonsterDTO
            }

            return Ok("No monsters found.");
        }
    }
}
