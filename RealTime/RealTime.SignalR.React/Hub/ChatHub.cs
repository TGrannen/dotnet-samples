using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RealTime.SignalR.React.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}