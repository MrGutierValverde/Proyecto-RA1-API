using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APIrpoyecto.Models
{
    public class MonsterDAOMemoria : ManagerInterface
    {
        public List<MonsterDTO> monsters = new List<MonsterDTO>();

        public MonsterDAOMemoria(List<MonsterDTO> monsters) { 
            this.monsters = monsters;
        }
        public MonsterDAOMemoria() { }
        public void Insert(MonsterDTO monster)
        {
            MonsterDTO monsterDTO = new MonsterDTO(monster.Id,
            monster.Name,
            monster.Type,
            monster.Hp,
            monster.Ac,
            monster.Url,
            monster.Cr,
            monster.Size,
            monster.Speed,
            monster.Align ,
            monster.Legendary,
            monster.Source,
            monster.Str,
            monster.Dex,
            monster.Con,
            monster.Intelligence,
            monster.Wis,
            monster.Cha);

            
            this.monsters.Add(monsterDTO);
        }
        public void Update(MonsterDTO monster) { 
            //uso single para que devuelva un solo objeto
            var col = monsters.Single(m => m.Id == monster.Id);
            //altero el objeto encontrado
            if (col != null) { 
                col.Name = monster.Name;
                col.Url = monster.Url;
                col.Cr = monster.Cr;
                col.Type = monster.Type;
                col.Size = monster.Size;
                col.Ac = monster.Ac;
                col.Hp = monster.Hp;
                col.Speed = monster.Speed;
                col.Align = monster.Align;
                col.Legendary = monster.Legendary;
                col.Source = monster.Source;
                col.Str = monster.Str;
                col.Dex = monster.Dex;
                col.Con = monster.Con;
                col.Intelligence = monster.Intelligence;
                col.Wis = monster.Wis;
                col.Cha = monster.Cha;
            }
            else
            {
                Console.WriteLine("No se ha encontrado el monstruo");
            }
        }
        public void Delete(int id) {
            MonsterDTO monster = new MonsterDTO(id);
            if (monsters.Contains(monster))
            {
                monsters.Remove(monster);
            }
            else
            {
                Console.WriteLine("No se ha encontrado el monstruo");
            }
        }
        public MonsterDTO GetById(int id)
        {
            MonsterDTO checkMonster = new MonsterDTO(id);
            if (monsters.Contains(checkMonster))
            {
                var monster = monsters.Single(m => m.Id == id);
                return monster;
            }
            else
            {
                Console.WriteLine("No se ha encontrado el monstruo");
                return checkMonster;
            }
        }

        public List<MonsterDTO> GetAll()
        {
            return monsters;
        }
        public void InsertMany(List<MonsterDTO> insertmonsters)
        {
            monsters = new List<MonsterDTO>();
            foreach (MonsterDTO monster in insertmonsters)
            {
                this.Insert(monster);
            }
        }
    }
}
