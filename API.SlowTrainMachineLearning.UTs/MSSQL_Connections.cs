using Microsoft.Data.SqlClient;

namespace API.SlowTrainMachineLearning.UTs
{
    public class MSSQL_Connections
    {
        [Fact]
        public void CheckConnection()
        {
            var connectionString =
            "Data Source=localhost,1500;" +
            "User ID=sa;" +
            "Password=Password_123#;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                Console.WriteLine("Połączenie działa!");

                using var command = new SqlCommand("SELECT @@VERSION", connection);
                var version = command.ExecuteScalar();

                Console.WriteLine("Wersja SQL Server:");
                Console.WriteLine(version);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd połączenia:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
