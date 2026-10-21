namespace RodeoBooking;

public record Service(int Id, string Name, int Minutes, int PriceDkk);
public record Stylist(int Id, string Name);
public record Booking(Guid Id, string Customer, DateOnly Day, TimeOnly Start, Stylist Stylist, Service Service)
{
    public TimeOnly End => Start.AddMinutes(Service.Minutes);
}
public record BookingResult(bool Success, string Message);

public sealed class BookingStore
{
    private readonly object gate = new();
    private readonly List<Booking> bookings = [];
    public static IReadOnlyList<Service> Services { get; } = Array.AsReadOnly(new[] {
        new Service(1, "Quick trim", 30, 250), new Service(2, "Cut & style", 60, 450), new Service(3, "Beard tidy", 30, 150)
    });
    public static IReadOnlyList<Stylist> Stylists { get; } = Array.AsReadOnly(new[] { new Stylist(1, "Alex"), new Stylist(2, "Sam") });
    public IReadOnlyList<Booking> ForDay(DateOnly day)
    {
        lock (gate) return bookings.Where(b => b.Day == day).OrderBy(b => b.Start).ToArray();
    }
    public BookingResult TryBook(string? customer, DateOnly day, TimeOnly start, int stylistId, int serviceId)
    {
        customer = customer?.Trim();
        var service = Services.SingleOrDefault(s => s.Id == serviceId);
        var stylist = Stylists.SingleOrDefault(s => s.Id == stylistId);
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (string.IsNullOrWhiteSpace(customer) || customer.Length > 60) return new(false, "Enter a name of 1–60 characters.");
        if (day < today || day > today.AddDays(90)) return new(false, "Choose a date within the next 90 days.");
        if (stylist is null || service is null) return new(false, "Choose a listed stylist and service.");
        if (start.Second != 0 || start.Millisecond != 0 || start.Ticks % TimeSpan.TicksPerSecond != 0 || start.Minute % 30 != 0 || start.Hour < 9 || start.Hour * 60 + start.Minute + service.Minutes > 17 * 60)
            return new(false, "Choose a half-hour slot between 09:00 and closing at 17:00.");
        var end = start.AddMinutes(service.Minutes);
        lock (gate)
        {
            if (bookings.Any(b => b.Day == day && b.Stylist.Id == stylistId && start < b.End && end > b.Start))
                return new(false, "That stylist is already booked during this time.");
            bookings.Add(new(Guid.NewGuid(), customer, day, start, stylist, service));
        }
        return new(true, "Your demo appointment is booked.");
    }
}
