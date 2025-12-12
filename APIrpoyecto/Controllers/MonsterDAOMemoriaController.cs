using APIrpoyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;

namespace APIrpoyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MonsterDAOMemoryController : ControllerBase
    {
        private readonly MonsterDAOMemoria _monsterDAOmemoria;

        // Constructor que recibe la instancia de MonsterDAOsql (Inyección de dependencias)
        public MonsterDAOMemoryController(MonsterDAOMemoria monsterDAOmemoria)
        {
            _monsterDAOmemoria = monsterDAOmemoria;
        }

        // Insertar un nuevo monstruo
        [HttpPost("Insert")]
        [Authorize(Roles = "Admin")]
        public IActionResult Insert([FromBody] MonsterDTO monster)
        {
            if (monster == null && monster.Name!=null)
            {
                return BadRequest("Monster data is required.");
            }

            _monsterDAOmemoria.Insert(monster);
            return Ok(true); // Petición exitosa
        }
        // Insertar varios monstruos
        [HttpPost("InsertMany")]
        [Authorize(Roles = "Admin")]
        public IActionResult InsertMany([FromBody] List<MonsterDTO> monsters)
        {
            if (monsters.Count<=0)
            {
                return BadRequest("Monster data is required.");
            }

            _monsterDAOmemoria.InsertMany(monsters);
            return Ok(true); // Petición exitosa
        }
        // Actualizar un monstruo existente
        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody] MonsterDTO monster)
        {
            if (monster == null && monster.Name!=null)
            {
                return BadRequest("Monster data is required.");
            }
            MonsterDTO has_monster = _monsterDAOmemoria.GetById(monster.Id);
            if (has_monster != null)
            {
                _monsterDAOmemoria.Update(monster);
                return Ok(true); // Petición exitosa
            }

            return NotFound("Monster not found.");
        }

        // Eliminar un monstruo por ID
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            MonsterDTO monster = _monsterDAOmemoria.GetById(id);
            if (monster != null && monster.Name != null)
            {
                _monsterDAOmemoria.Delete(id);
                return Ok(true); // Devolver objeto MonsterDTO
            }
            return NotFound("Monster not found.");
        }

        // Obtener un monstruo por ID
        [HttpGet("GetById/{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            MonsterDTO monster = _monsterDAOmemoria.GetById(id);
            if (monster != null && monster.Name!=null)
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
            List<MonsterDTO> monsters = _monsterDAOmemoria.GetAll();
            if (monsters != null && monsters.Count > 0)
            {
                return Ok(monsters); // Devolver lista de MonsterDTO
            }

            return Ok("No monsters found.");
        }
    }
}
