using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NET5SignalR.Models
{
    public class NoticeHub : Hub<IHubClient>
    {
        public async Task SendNotice(Notice notice)
        {
            await Clients.All.ChangeNotice( notice);
        }
    }
}
