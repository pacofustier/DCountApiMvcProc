using DCountApiMvcProc.Models;
using DCountApiMvcProc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DCountApiMvcProc.Controllers;

[Route("[controller]")]
[ApiController]
public class WorkerController(WorkerService workerService) : ControllerBase
{
    private readonly WorkerService _workerService = workerService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Worker>))]
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

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Worker))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Worker>> GetWorkerById(
        [FromRoute] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero.");

            var worker = await _workerService.GetWorkerById(id);
            if (worker == null)
            {
                return NotFound($"Worker with ID {id} not found.");
            }
            return Ok(worker);

        }
        catch (SqlException sqlEx)
        {
            return BadRequest(sqlEx.Message);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Worker))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Worker>> CreateWorker(
        [FromBody] Worker worker)
    {
        try
        {
            if (worker is null)
                return BadRequest("The body can't null");
            if(string.IsNullOrWhiteSpace(worker.Name))
                return BadRequest("Worker Name is required");

            var createdWorker = await _workerService.CreateWorker(worker);
            if (createdWorker == null)
                return StatusCode(500, "Created worker failure");

            return CreatedAtAction(nameof(GetWorkerById), new { id = createdWorker.Id }, createdWorker);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Worker))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Worker>> UpdateWorker(
        [FromRoute] int id, 
        [FromBody] Worker worker)
    {
        try
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero.");
            if (worker is null)
                return BadRequest("The body can't null");
            if (string.IsNullOrWhiteSpace(worker.Name))
                return BadRequest("Worker Name is required");

            var updated = await _workerService.UpdateWorker(id, worker.Name, worker.Email);
            if (updated is null)
                return NotFound($"Aucun Worker trouvé avec l'Id {id}");

            return Ok(updated);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Worker))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteWorker(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero.");
            var result = await _workerService.DeleteWorker(id);
            if (result is null)
                return NotFound($"Aucun Worker trouvé avec l'Id {id}");

            return Ok(result);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }
}
