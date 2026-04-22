using Microsoft.AspNetCore.Mvc;
using TicketService.Data;
using TicketService.Models;
using System.Text.Json;

namespace TicketService.Controllers
{
    [ApiController]
    [Route("tickets")]
    public class TicketController : ControllerBase
    {
        private readonly TicketDbContext _context;
        private readonly HttpClient _httpClient;

        public TicketController(TicketDbContext context, IHttpClientFactory factory)
        {
            _context = context;
            _httpClient = factory.CreateClient();
        }

        // Create new ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            // Call Event Service to check if event exists
            var response = await GetEvent(ticket.EventId);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Event does not exist");
            }

            var json = await response.Content.ReadAsStringAsync();

            var eventObj = JsonSerializer.Deserialize<EventDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

        // Get all tickets
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = _context.Tickets.ToList();

            var result = new List<TicketResponseDto>();

            foreach (var t in tickets)
            {
                // Call Event Service to get event details
                var response = await GetEvent(t.EventId);

                if (!response.IsSuccessStatusCode)
                {
                    result.Add(new TicketResponseDto
                    {
                        Id = t.Id,
                        EventId = t.EventId,
                        BuyerName = t.BuyerName,
                        EventName = "Event not found"
                    });

                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                var eventObj = JsonSerializer.Deserialize<EventDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                result.Add(new TicketResponseDto
                {
                    Id = t.Id,
                    EventId = t.EventId,
                    BuyerName = t.BuyerName,
                    EventName = eventObj.Name
                });
            }

            return Ok(result);
        }
        // Delete Ticket
        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
                return NotFound("Ticket not found");

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            return NoContent();
        }

        // delete all tickets for an event
        [HttpDelete("eventTicket/{eventId}")]
        public IActionResult DeleteTicketsByEvent(int eventId)
        {
            var tickets = _context.Tickets.Where(t => t.EventId == eventId).ToList();

            _context.Tickets.RemoveRange(tickets);
            _context.SaveChanges();

            return NoContent();
        }

        private async Task<HttpResponseMessage> GetEvent(int eventId)
        {
            return await _httpClient.GetAsync($"http://eventservice:80/events/{eventId}");

        }
    }
}