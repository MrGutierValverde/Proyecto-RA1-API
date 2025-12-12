using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIrpoyecto.Models
{
    public class MonsterDTO
    {
        //contador para la autogeneración de id
        private static int nextId = 1;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Cr { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public int Ac { get; set; }
        public int Hp { get; set; }
        public string Speed { get; set; }
        public string Align { get; set; }
        public bool Legendary { get; set; }
        public string Source { get; set; }
        public string Str { get; set; }
        public string Dex { get; set; }
        public string Con { get; set; }
        public string Intelligence { get; set; }
        public string Wis { get; set; }
        public string Cha { get; set; }
        
        public MonsterDTO() { }
        //Constructor para las comparaciones equals
        public MonsterDTO(int id)
        {
            this.Id = id;
        }
        //Constructor con autogeneración de ID
        public MonsterDTO(
            string name,
            string type,
            int hp,
            int ac,
            string url,
            string cr ,
            string size ,
            string speed ,
            string align ,
            bool legendary ,
            string source ,
            string str,
            string dex,
            string con,
            string intelligence,
            string wis ,
            string cha)
        {
            // ID autogenerada
            this.Id = nextId++;

            this.Name = name;
            this.Type = type;
            this.Hp = hp;
            this.Ac = ac;
            this.Url = url;
            this.Cr = cr;
            this.Size = size;
            this.Speed = speed;
            this.Align = align;
            this.Legendary = legendary;
            this.Source = source;
            this.Str = str;
            this.Dex = dex;
            this.Con = con;
            this.Intelligence = intelligence;
            this.Wis = wis;
            this.Cha = cha;
        }

        //Constructor con ID manual (para cargar desde BD)
        public MonsterDTO(
            int id,
            string name,
            string type,
            int hp,
            int ac,
            string url,
            string cr,
            string size,
            string speed,
            string align ,
            bool legendary,
            string source,
            string str,
            string dex,
            string con,
            string intelligence,
            string wis,
            string cha)
        {
            // Para evitar duplicidad de IDs
            if (id >= nextId)
            {
                nextId = id + 1;
                this.Id = id;
            }
            else {
                this.Id = nextId;
                nextId++;
            }
            this.Name = name;
            this.Type = type;
            this.Hp = hp;
            this.Ac = ac;
            this.Url = url;
            this.Cr = cr;
            this.Size = size;
            this.Speed = speed;
            this.Align = align;
            this.Legendary = legendary;
            this.Source = source;
            this.Str = str;
            this.Dex = dex;
            this.Con = con;
            this.Intelligence = intelligence;
            this.Wis = wis;
            this.Cha = cha;
        }

        public override bool Equals(object? obj)
        {
            // Si es el mismo objeto en memoria
            if (ReferenceEquals(this, obj))
                return true;

            // Si es null o de otro tipo
            if (obj is not MonsterDTO other)
                return false;

            // Si es valido se comprueba la id
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            // Uso el Id como base del hash
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{Id}] {Name}");
            sb.AppendLine($"  Type: {Type} | Size: {Size} | CR: {Cr}");
            sb.AppendLine($"  AC: {Ac} | HP: {Hp} | Speed: {Speed}");
            sb.AppendLine($"  Alignment: {Align} | Legendary: {(Legendary ? "Yes" : "No")}");
            sb.AppendLine($"  STR: {Str}, DEX: {Dex}, CON: {Con}, INT: {Intelligence}, WIS: {Wis}, CHA: {Cha}");
            sb.AppendLine($"  Source: {Source}");
            sb.AppendLine($"  URL: {Url}");
            return sb.ToString();
        }

    }
}
