using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RodeoBooking.Pages;
public class IndexModel(BookingStore store) : PageModel
{
    [BindProperty, Required, StringLength(60)] public string Customer { get; set; } = "";
    [BindProperty(SupportsGet = true)] public DateOnly Day { get; set; } = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
    [BindProperty] public string Start { get; set; } = "10:00";
    [BindProperty] public int StylistId { get; set; } = 1;
    [BindProperty] public int ServiceId { get; set; } = 1;
    [TempData] public string? Notice { get; set; }
    public IReadOnlyList<Booking> Bookings => store.ForDay(Day);
    public void OnGet() { }
    public IActionResult OnPost()
    {
        if (!TimeOnly.TryParseExact(Start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
            ModelState.AddModelError(nameof(Start), "Choose a valid time.");
        if (!ModelState.IsValid) return Page();
        var result = store.TryBook(Customer, Day, start, StylistId, ServiceId);
        if (!result.Success) { ModelState.AddModelError("", result.Message); return Page(); }
        Notice = result.Message;
        return RedirectToPage(new { Day = Day.ToString("yyyy-MM-dd") });
    }
}
