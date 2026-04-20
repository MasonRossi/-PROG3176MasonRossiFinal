using System.Text.Json.Serialization;
namespace TicketService.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string BuyerName { get; set; }

    }
}