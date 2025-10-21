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

    public async Task<Worker?> GetWorkerById(int id)
    {
        Worker? worker = null;
        using (SqlConnection conn = new(_connectionString))
        using (SqlCommand cmd = new("dbo.usp_GetWorkerById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                worker = new Worker
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email"))
                };
            }
        }
        return worker;
    }

    public async Task<Worker?> CreateWorker(Worker worker)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand("dbo.usp_CreateWorker", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", worker.Name);
            cmd.Parameters.AddWithValue("@Email", (object?)worker.Email ?? DBNull.Value);

            await conn.OpenAsync();

            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Worker
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("NewId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("Email"))
                    };
                }
            }
        }

        return null;
    }

    public async Task<Worker?> UpdateWorker(int id, string? name, string? email)
    {
        using SqlConnection conn = new(_connectionString);
        using SqlCommand cmd = new("dbo.usp_UpdateWorker", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Name", (object?)name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);

        await conn.OpenAsync();

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Worker
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("Email"))
            };
        }

        return null;
    }

    public async Task<(int IdRemoved, string Message)?> DeleteWorker(int id)
{
    using (SqlConnection conn = new SqlConnection(_connectionString))
    using (SqlCommand cmd = new SqlCommand("dbo.usp_DeleteWorker", conn))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();

        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32(reader.GetOrdinal("IdRemoved")),
                    reader.GetString(reader.GetOrdinal("Message"))
                );
            }
        }
    }

    return null;
}

}
