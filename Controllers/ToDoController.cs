using ASP.NETCoreWebAPI_Sample.Entitys;  // Verwende das richtige ToDoItem-Modell
using ASP.NETCoreWebAPI_Sample.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NETCoreWebAPI_Sample.Controllers  // Der richtige Namensraum für den Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly string _apiKey = "AMSecretKey16";

        // Konstruktor, um den DbContext zu injizieren
        public ToDoController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // HTTP-GET-Methode, um alle ToDo-Elemente zurückzugeben
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> Get([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                // Überprüfe den API-Schlüssel aus dem Header
                if (apiKey != _apiKey)
                {
                    return Unauthorized("Invalid API Key");
                }

                var items = await _dbContext.ToDoItems.ToListAsync();  // Datenbankabfrage
                return Ok(items);
            }
            catch (Exception ex)
            {
                // Logge den Fehler und gib eine Fehlermeldung zurück
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");  // Gib eine 500-Fehlerantwort zurück
            }
        }


        // HTTP-GET-Methode, die ein spezifisches ToDo-Element nach der ID zurückgibt
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItem>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
            }

            var item = await _dbContext.ToDoItems.FindAsync(id);  // Holt das ToDoItem nach ID aus der Datenbank
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // HTTP-POST-Methode, um ein neues ToDo-Element hinzuzufügen
        [HttpPost]
        public IActionResult AddToDoItem([FromBody] ToDoItem todoItem)
        {
            if (string.IsNullOrWhiteSpace(todoItem.Task) || todoItem.Task.Length > 100)
            {
                return BadRequest("Task cannot be empty or exceed 100 characters.");
            }

            // Task is now safe to be added to the database
            _dbContext.ToDoItems.Add(todoItem);
            _dbContext.SaveChanges();

            return Ok("ToDo item added successfully");
        }


        // HTTP-PUT-Methode, um ein bestehendes ToDo-Element zu aktualisieren
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ToDoItem updatedItem)
        {
            var item = await _dbContext.ToDoItems.FindAsync(id);  // Holt das ToDoItem nach ID aus der Datenbank
            if (item == null)
            {
                return NotFound();
            }

            item.Task = updatedItem.Task;  // Aktualisiere den Task-Namen
            item.IsCompleted = updatedItem.IsCompleted;  // Aktualisiere den Status

            await _dbContext.SaveChangesAsync();  // Speichern der Änderungen in der Datenbank

            return NoContent();
        }

        // HTTP-DELETE-Methode, um ein ToDo-Element nach der ID zu löschen
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _dbContext.ToDoItems.FindAsync(id);  // Holt das ToDoItem nach ID aus der Datenbank
            if (item == null)
            {
                return NotFound();
            }

            _dbContext.ToDoItems.Remove(item);  // Entferne das ToDoItem aus der Datenbank
            await _dbContext.SaveChangesAsync();  // Speichern der Änderungen in der Datenbank

            return NoContent();
        }
    }
}
