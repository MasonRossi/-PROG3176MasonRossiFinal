using System.Net.Http.Json;

var httpClient = new HttpClient();

var eventServiceUrl = "http://localhost:5200/events";
var ticketServiceUrl = "http://localhost:5116/tickets";
Console.WriteLine("Starting Client App...\n");
while (true)
{
    Console.WriteLine("\n----- Ticket App Client -----");
    Console.WriteLine("1. Create Event");
    Console.WriteLine("2. Create Ticket");
    Console.WriteLine("3. View Events");
    Console.WriteLine("4. View Tickets");
    Console.WriteLine("5. Delete Event");
    Console.WriteLine("6. Delete Ticket");
    Console.WriteLine("7. Exit");
    Console.Write("Select option: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await CreateEvent(httpClient, eventServiceUrl);
            Console.ReadLine();
            break;

        case "2":
            await CreateTicket(httpClient, ticketServiceUrl);
            Console.ReadLine();
            break;

        case "3":
            await ViewEvents(httpClient, eventServiceUrl);
            Console.ReadLine();
            break;

        case "4":
            await ViewTickets(httpClient, ticketServiceUrl);
            Console.ReadLine();
            break;

        case "5":
            await DeleteEvent(httpClient, eventServiceUrl);
            Console.ReadLine();
            break;

        case "6":
            await DeleteTicket(httpClient, ticketServiceUrl);
            Console.ReadLine();
            break;

        case "7":
            return;

        default:
            Console.WriteLine("Invalid option");
            break;
    }
}

// Console helpers
// Create an event
static async Task CreateEvent(HttpClient client, string url)
{
    Console.Write("Event Name: ");
    var name = Console.ReadLine();

    Console.Write("Location: ");
    var location = Console.ReadLine();

    var newEvent = new
    {
        Name = name,
        Location = location,
        Date = DateTime.Now,
        Capacity = 100
    };

    var response = await client.PostAsJsonAsync(url, newEvent);

    Console.WriteLine(response.IsSuccessStatusCode
        ? "Event created"
        : "Failed to create event");
}

// Create new ticket
static async Task CreateTicket(HttpClient client, string url)
{
    Console.Write("Event ID: ");
    if (!int.TryParse(Console.ReadLine(), out var eventId))
    {
        Console.WriteLine("Invalid event ID");
        return;
    }
    Console.Write("Buyer Name: ");
    var buyer = Console.ReadLine();

    var ticket = new
    {
        EventId = eventId,
        BuyerName = buyer
    };

    var response = await client.PostAsJsonAsync(url, ticket);

    var result = await response.Content.ReadAsStringAsync();

   Console.WriteLine(response.IsSuccessStatusCode
        ? "Ticket created"
        : "Failed to create ticket: ");
}

// View all events
static async Task ViewEvents(HttpClient client, string url)
{
    var events = await client.GetFromJsonAsync<List<EventDto>>(url);

    if (events == null || events.Count == 0)
    {
        Console.WriteLine("No events found");
        return;
    }

    Console.WriteLine("\n----- EVENTS -----");

    foreach (var e in events)
    {
        Console.WriteLine($"ID: {e.Id}");
        Console.WriteLine($"Name: {e.Name}");
        Console.WriteLine($"Location: {e.Location}");
        Console.WriteLine("----------------------");
    }
}

// View all tickets
static async Task ViewTickets(HttpClient client, string url)
{
    var tickets = await client.GetFromJsonAsync<List<TicketResponseDto>>(url);

    if (tickets == null || tickets.Count == 0)
    {
        Console.WriteLine("No tickets found");
        return;
    }

    Console.WriteLine("\n----- TICKETS -----");

    foreach (var t in tickets)
    {
        Console.WriteLine($"Ticket ID: {t.Id}");
        Console.WriteLine($"Event Name: {t.EventName}");
        Console.WriteLine($"Buyer: {t.BuyerName}");
        Console.WriteLine("----------------------");
    }
}

// Delete an event
static async Task DeleteEvent(HttpClient client, string url)
{
    Console.Write("Event ID to delete: ");
    var id = Console.ReadLine();

    var response = await client.DeleteAsync($"{url}/{id}");

    Console.WriteLine(response.IsSuccessStatusCode
        ? "Event deleted (and tickets cleaned up)"
        : "Failed to delete event");
}

// Delete a ticket
static async Task DeleteTicket(HttpClient client, string url)
{
    Console.Write("Ticket ID to delete: ");
    var id = Console.ReadLine();

    var response = await client.DeleteAsync($"{url}/{id}");

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Ticket deleted successfully");
    }
    else
    {
        Console.WriteLine("Failed to delete ticket");
    }
}

