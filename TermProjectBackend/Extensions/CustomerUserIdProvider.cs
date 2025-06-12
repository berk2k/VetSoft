using Microsoft.AspNetCore.SignalR;
using TermProjectBackend.Extensions;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var userId = connection.User?.GetUserId();
        return userId?.ToString();
    }
}
