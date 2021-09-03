using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace NET5SignalR.Models
{
    public class NoticeHub : Hub<IHubClient>
    {
        private readonly StockDBContext _context;
        public NoticeHub(StockDBContext context)
        {
            _context = context;
        }

        //public async Task SendNotice(Notice notice)
        //{
        //    await Clients.All.ChangeNotice( notice);
        //}

        public void AddStock(Stock stock)
        {
            var context = Context;
            var client = Clients;
            var s = _context.Stocks.Where(s => s.Symbol == stock.Symbol).FirstOrDefault();
            if(s == null)
            {
                this._context.Stocks.Add(stock);
                this._context.SaveChanges();
            }
        }
    }
}
