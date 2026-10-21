# Rodeo Booking — C# / ASP.NET Core Razor Pages

A small salon appointment application: choose a treatment, stylist, date and time; see the day's bookings; reject overlapping appointments.

**New AI-assisted reconstruction**, inspired by the Rodeo / `Booking_Gruppe` study-project idea discussed in November–December 2023. The original complete source was not recovered. This repository is a new implementation, with a smaller scope, and does not claim to reproduce the original group members' contributions. Rodeo is used as the historical project label, not as a real salon integration.

## Run

.NET 10 SDK required; no database or frontend package installation.

```sh
dotnet run --urls http://127.0.0.1:5081
```

Open http://127.0.0.1:5081. Use fictional first names. Appointments exist in memory and disappear on restart. The page is a shared local demonstration, with no customer accounts or private booking area.

## Implementation

- Razor Pages, model binding, validation and post/redirect/get.
- Default ASP.NET Core antiforgery protection and Razor output encoding.
- Typed `DateOnly` / `TimeOnly` domain model and treatment durations.
- Half-open time intervals: a 10:00–11:00 appointment allows an 11:00 start.
- Per-stylist overlap checks and a lock making check-and-insert atomic in this process.
- Separate booking rules and responsive server-rendered UI.

```sh
dotnet test tests/RodeoBooking.Tests.csproj
```

Five tests cover overlaps/adjacency, closing time, invalid inputs, simultaneous booking requests, page rendering and missing antiforgery tokens. The in-memory lock is intentionally limited to one application process; a production system would need persistent storage and database-level booking constraints.
