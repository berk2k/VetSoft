using Microsoft.AspNetCore.SignalR;

namespace TermProjectBackend.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, "VetStaff");
            Console.WriteLine($"Veteriner web client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "VetStaff");
            Console.WriteLine($"Veteriner web client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        
        public async Task SendMessage(string message)
        {
            Console.WriteLine("message sent from signalR: " + message);
            await Clients.Group("VetStaff").SendAsync("ReceiveNotification", message);
        }
    }
}
