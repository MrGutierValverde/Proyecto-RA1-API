using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using APIrpoyecto.Models;
namespace APIrpoyecto.Models
{
    public class MonsterDAOsql : ManagerInterface
    {
        private string ConnectionString { get; set; }

        public MonsterDAOsql()
        {

        }
        public MonsterDAOsql(string connection)
        {
            this.ConnectionString = connection;
        }
        public void Insert(MonsterDTO monster)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Monsters 
                (Name, Url, Cr, Type, Size, Ac, Hp, Speed, Align, Legendary, Source, Str, Dex, Con, Intelligence, Wis, Cha) 
                VALUES 
                (@Name, @Url, @Cr, @Type, @Size, @Ac, @Hp, @Speed, @Align, @Legendary, @Source, @Str, @Dex, @Con, @Intelligence, @Wis, @Cha)";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", monster.Name);
                        cmd.Parameters.AddWithValue("@Url", monster.Url);
                        cmd.Parameters.AddWithValue("@Cr", monster.Cr);
                        cmd.Parameters.AddWithValue("@Type", monster.Type);
                        cmd.Parameters.AddWithValue("@Size", monster.Size);
                        cmd.Parameters.AddWithValue("@Ac", monster.Ac);
                        cmd.Parameters.AddWithValue("@Hp", monster.Hp);
                        cmd.Parameters.AddWithValue("@Speed", monster.Speed);
                        cmd.Parameters.AddWithValue("@Align", monster.Align);
                        cmd.Parameters.AddWithValue("@Legendary", monster.Legendary);
                        cmd.Parameters.AddWithValue("@Source", monster.Source);
                        cmd.Parameters.AddWithValue("@Str", monster.Str);
                        cmd.Parameters.AddWithValue("@Dex", monster.Dex);
                        cmd.Parameters.AddWithValue("@Con", monster.Con);
                        cmd.Parameters.AddWithValue("@Intelligence", monster.Intelligence);
                        cmd.Parameters.AddWithValue("@Wis", monster.Wis);
                        cmd.Parameters.AddWithValue("@Cha", monster.Cha);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Verificar si el error es una violación de la clave única
                if (ex.Number == 1062) // Error de duplicado de clave única
                {
                    Console.WriteLine("Error: El nombre del monstruo ya existe.");
                }
                else
                {
                    // Otros errores de MySQL
                    Console.WriteLine($"Error de base de datos: {ex.Message}");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Update(MonsterDTO monster)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Monsters SET 
                Name=@Name, Url=@Url, Cr=@Cr, Type=@Type, Size=@Size, Ac=@Ac, Hp=@Hp, 
                Speed=@Speed, Align=@Align, Legendary=@Legendary, Source=@Source,
                Str=@Str, Dex=@Dex, Con=@Con, Intelligence=@Intelligence, Wis=@Wis, Cha=@Cha
                WHERE Id=@Id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", monster.Id);
                        cmd.Parameters.AddWithValue("@Name", monster.Name);
                        cmd.Parameters.AddWithValue("@Url", monster.Url);
                        cmd.Parameters.AddWithValue("@Cr", monster.Cr);
                        cmd.Parameters.AddWithValue("@Type", monster.Type);
                        cmd.Parameters.AddWithValue("@Size", monster.Size);
                        cmd.Parameters.AddWithValue("@Ac", monster.Ac);
                        cmd.Parameters.AddWithValue("@Hp", monster.Hp);
                        cmd.Parameters.AddWithValue("@Speed", monster.Speed);
                        cmd.Parameters.AddWithValue("@Align", monster.Align);
                        cmd.Parameters.AddWithValue("@Legendary", monster.Legendary);
                        cmd.Parameters.AddWithValue("@Source", monster.Source);
                        cmd.Parameters.AddWithValue("@Str", monster.Str);
                        cmd.Parameters.AddWithValue("@Dex", monster.Dex);
                        cmd.Parameters.AddWithValue("@Con", monster.Con);
                        cmd.Parameters.AddWithValue("@Intelligence", monster.Intelligence);
                        cmd.Parameters.AddWithValue("@Wis", monster.Wis);
                        cmd.Parameters.AddWithValue("@Cha", monster.Cha);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Verificar si el error es una violación de la clave única
                if (ex.Number == 1062) // Error de duplicado de clave única
                {
                    Console.WriteLine("Error: El nombre del monstruo ya existe.");
                }
                else
                {
                    // Otros errores de MySQL
                    Console.WriteLine($"Error de base de datos: {ex.Message}");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Monsters WHERE Id=@Id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {

            }

        }

        public MonsterDTO GetById(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Monsters WHERE Id=@Id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MonsterDTO monster = new MonsterDTO(reader.GetInt32("Id"))
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    Url = reader.GetString("Url"),
                                    Cr = reader.GetString("Cr"),
                                    Type = reader.GetString("Type"),
                                    Size = reader.GetString("Size"),
                                    Ac = reader.GetInt32("Ac"),
                                    Hp = reader.GetInt32("Hp"),
                                    Speed = reader.GetString("Speed"),
                                    Align = reader.GetString("Align"),
                                    Legendary = reader.GetBoolean("Legendary"),
                                    Source = reader.GetString("Source"),
                                    Str = reader.GetInt32("Str") + "",
                                    Dex = reader.GetInt32("Dex") + "",
                                    Con = reader.GetInt32("Con") + "",
                                    Intelligence = reader.GetInt32("Intelligence") + "",
                                    Wis = reader.GetInt32("Wis") + "",
                                    Cha = reader.GetInt32("Cha") + ""
                                };
                                return monster;
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {

            }
            return null;
        }

        public List<MonsterDTO> GetAll()
        {
            try
            {
                var monsters = new List<MonsterDTO>();

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Monsters";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            monsters.Add(new MonsterDTO(reader.GetInt32("Id"))
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Url = reader.GetString("Url"),
                                Cr = reader.GetString("Cr"),
                                Type = reader.GetString("Type"),
                                Size = reader.GetString("Size"),
                                Ac = reader.GetInt32("Ac"),
                                Hp = reader.GetInt32("Hp"),
                                Speed = reader.GetString("Speed"),
                                Align = reader.GetString("Align"),
                                Legendary = reader.GetBoolean("Legendary"),
                                Source = reader.GetString("Source"),
                                Str = reader.GetInt32("Str") + "",
                                Dex = reader.GetInt32("Dex") + "",
                                Con = reader.GetInt32("Con") + "",
                                Intelligence = reader.GetInt32("Intelligence") + "",
                                Wis = reader.GetInt32("Wis") + "",
                                Cha = reader.GetInt32("Cha") + ""
                            });
                        }
                    }
                }

                return monsters;
            }
            catch (Exception ex) {
                return null;
            }
        }
        public void InsertMany(List<MonsterDTO> monsters)
        {
            foreach(MonsterDTO monster in monsters)
            {
                this.Insert(monster);
            }
        }
    }
}
