using Microsoft.AspNetCore.Mvc.Testing;
using RodeoBooking;
using Xunit;

public class BookingTests
{
    private static DateOnly Tomorrow => DateOnly.FromDateTime(DateTime.Today).AddDays(1);
    [Fact] public void AdjacentAppointmentsAreAllowedButOverlapsAreRejected()
    {
        var store = new BookingStore();
        Assert.True(store.TryBook("Demo", Tomorrow, new(10, 0), 1, 2).Success);
        Assert.False(store.TryBook("Demo", Tomorrow, new(10, 30), 1, 1).Success);
        Assert.True(store.TryBook("Demo", Tomorrow, new(11, 0), 1, 1).Success);
        Assert.True(store.TryBook("Demo", Tomorrow, new(10, 0), 2, 2).Success);
    }
    [Fact] public void DurationCannotExtendPastClosingTime()
    {
        var store = new BookingStore();
        Assert.False(store.TryBook("Demo", Tomorrow, new(16, 30), 1, 2).Success);
        Assert.True(store.TryBook("Demo", Tomorrow, new(16, 30), 1, 1).Success);
        Assert.False(store.TryBook("Demo", Tomorrow, new(9, 15), 1, 1).Success);
    }
    [Fact] public void InvalidNamesDatesAndCatalogIdsAreRejected()
    {
        var store = new BookingStore();
        Assert.False(store.TryBook("  ", Tomorrow, new(10, 0), 1, 1).Success);
        Assert.False(store.TryBook("Demo", Tomorrow.AddDays(-2), new(10, 0), 1, 1).Success);
        Assert.False(store.TryBook("Demo", Tomorrow.AddDays(91), new(10, 0), 1, 1).Success);
        Assert.False(store.TryBook("Demo", Tomorrow, new(10, 0), 99, 1).Success);
    }
    [Fact] public void ConcurrentRequestsCannotDoubleBookOneStylist()
    {
        var store = new BookingStore();
        var results = new bool[30];
        Parallel.For(0, results.Length, i => results[i] = store.TryBook("Demo", Tomorrow, new(10, 0), 1, 1).Success);
        Assert.Equal(1, results.Count(success => success));
    }
    [Fact] public async Task RazorPageLoadsAndRejectsPostsWithoutAntiforgeryToken()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();
        Assert.Contains("Make an appointment", await client.GetStringAsync("/"));
        var response = await client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>()));
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
