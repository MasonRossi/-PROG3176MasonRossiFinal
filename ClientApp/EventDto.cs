using System.ComponentModel.DataAnnotations;

public class EventDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required int Capacity { get; set; }
}