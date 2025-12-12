using System.Collections.Generic;
using APIrpoyecto.Models; // reutiliza MonsterDAOsql y MonsterDTO

namespace MCPServer
{
    public class MonsterMcpService
    {
        private readonly MonsterDAOsql _dao;
        public MonsterMcpService(MonsterDAOsql dao)
        {
            _dao = dao;
        }

        public List<MonsterDTO> GetAll()
        {
            return _dao.GetAll();
        }

        public MonsterDTO GetById(int id)
        {
            return _dao.GetById(id);
        }

        public List<MonsterDTO> SearchByName(string name)
        {
            var all = _dao.GetAll();
            if (all == null) return new List<MonsterDTO>();
            return all.Where(m => m.Name != null && m.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
    }
}
