using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Models;

namespace EventService.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventController : ControllerBase
    {
        private readonly EventDbContext _context;
        private readonly HttpClient _httpClient;
        public EventController(EventDbContext context, IHttpClientFactory factory)
        {
            _context = context;
            _httpClient = factory.CreateClient();
        }

        // Get all events
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }

        // Create new event
        [HttpPost]
        public IActionResult CreateEvent(Event ev)
        {
            _context.Events.Add(ev);
            _context.SaveChanges();
            return Ok(ev);
        }

        //Get event by id
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        // Delete Event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = _context.Events.Find(id);

            if (ev == null)
                return NotFound("Event not found");

            await _httpClient.DeleteAsync($"http://localhost:5116/tickets/eventTicket/{id}");

            _context.Events.Remove(ev);
            _context.SaveChanges();

            return NoContent();
        }
    }
}