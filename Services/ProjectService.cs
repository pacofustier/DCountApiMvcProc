using System.Data;
using DCountApiMvcProc.Models;
using Microsoft.Data.SqlClient;

namespace DCountApiMvcProc.Services;

public class ProjectService(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' not found.");

    public async Task<List<Project>> GetAllProjects()
    {
        var projects = new List<Project>();

        using (SqlConnection conn = new(_connectionString))
        using (SqlCommand cmd = new("dbo.usp_GetAllProjects", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                projects.Add(new Project
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description"))
                });
            }
        }
        return projects;
    }

    public async Task<Project?> GetProjectById(int id)
    {
        Project? project = null;
        using (SqlConnection conn = new(_connectionString))
        using (SqlCommand cmd = new("dbo.usp_GetProjectById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                project = new Project
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description"))
                };
            }
        }
        return project;
    }
}
