using DCountApiMvcProc.Models;
using DCountApiMvcProc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace DCountApiMvcProc.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkerController(WorkerService workerService) : ControllerBase
{
    private readonly WorkerService _workerService = workerService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Worker>>> GetAllWorkers()
    {
        try
        {             
            var workers = await _workerService.GetAllWorkers();
            return Ok(workers);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Worker>> GetWorkerById(
        [FromRoute] int id)
    {
        try
        {
            var worker = await _workerService.GetWorkerById(id);
            if (worker == null)
            {
                return NotFound($"Worker with ID {id} not found.");
            }
            return Ok(worker);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateWorker(
        [FromBody] Worker worker)
    {
        try
        {
            var newWorkerId = await _workerService.CreateWorker(worker);
            return CreatedAtAction(nameof(GetWorkerById), new { id = newWorkerId }, newWorkerId);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Worker>> UpdateWorker(int id, [FromBody] Worker worker)
    {
        try
        {
            var updated = await _workerService.UpdateWorker(id, worker.Name, worker.Email);
            if (updated == null)
                return NotFound($"Aucun Worker trouvé avec l'Id {id}");

            return Ok(updated);
        }
        catch (SqlException ex)
        {
            // Gestion des erreurs levées par THROW dans SQL
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }
}
