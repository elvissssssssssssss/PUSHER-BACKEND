namespace apitextil
{
    using MySql.Data.MySqlClient;
    using System;
    using System.Data;

    public class DatabaseConnectionTester
    {
        private readonly string _connectionString;

        public DatabaseConnectionTester(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Conexión exitosa a la base de datos.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectarse a la base de datos: {ex.Message}");
                return false;
            }
        }
    }
}
