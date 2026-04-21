public class TicketResponseDto
{
    public int Id { get; set; }
    public required int EventId { get; set; }
    public required string BuyerName { get; set; }
    public required string EventName { get; set; }
}