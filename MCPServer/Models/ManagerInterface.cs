using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIrpoyecto.Models
{
    public interface ManagerInterface
    {
        void Insert(MonsterDTO monster);
        void Update(MonsterDTO monster);
        void Delete(int id);
        MonsterDTO GetById(int id);
        List<MonsterDTO> GetAll();

    }
}
