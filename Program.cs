using RodeoBooking;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<BookingStore>();
var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
public partial class Program { }
