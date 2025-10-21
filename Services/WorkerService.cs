namespace DCountApiMvcProc.Services;

using System.Data;
using DCountApiMvcProc.Models;
using Microsoft.Data.SqlClient;

public class WorkerService(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' not found.");

    public async Task<List<Worker>> GetAllWorkers()
    {
        var workers = new List<Worker>();

        using (SqlConnection conn = new(_connectionString))
        using (SqlCommand cmd = new("dbo.usp_GetAllWorkers", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                workers.Add(new Worker
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email"))
                });
            }
        }

        return workers;
    }
}
