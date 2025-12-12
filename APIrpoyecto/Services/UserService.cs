using APIrpoyecto.Controllers;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace APIrpoyecto.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public User Authenticate(string username, string password)
        {   string storedpassword = "";
            string user = "";
            string role = "";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = "SELECT UserName, Password, Role FROM Users WHERE UserName = @Username";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = reader.GetString("UserName");
                            storedpassword = reader.GetString("Password");
                            role = reader.GetString("Role");
                        }
                    }
                }
            }
            if (user != null&& user!="" && VerifyPassword(password, storedpassword))
                {
                    return new User(user,password,role); // Si la contraseña es correcta, devolvemos el objeto del usuario
                }

            return null; // Usuario no encontrado o contraseña incorrecta
        }
        // Verificar si el usuario ya existe
        public bool CheckIfUserExists(string username)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Users WHERE UserName = @Username";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Registrar un nuevo usuario
        public User RegisterUser(string username, string passwordHash, string role)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO Users (UserName, Password, Role) VALUES (@Username, @Password, @Role)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", passwordHash);
                    cmd.Parameters.AddWithValue("@Role", role);

                    var result = cmd.ExecuteNonQuery();
                    if (result > 0)  // Si la inserción fue exitosa
                    {
                        return new User(username, passwordHash, role);  // Devolver el usuario recién creado
                    }
                }
            }

            return null; // Error al registrar el usuario
        }
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public User() { }

            public User(string username,string password,string role)
            {
                this.Username = username;
                this.Password = password;
                this.Role = role;
            }
        }
        // Método para generar el hash SHA-256 de la contraseña
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);  // Convertir a Base64 para almacenar en la base de datos
            }
        }

        // Método para verificar si la contraseña proporcionada coincide con el hash almacenado
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword == storedHash;  // Comparar los hashes
        }
    }

}
