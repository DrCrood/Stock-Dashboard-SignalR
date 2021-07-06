using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NET5SignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET5SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockDBContext _context;
        private readonly IHubContext<NoticeHub, IHubClient> _hubContext;


        public StockController(StockDBContext context, IHubContext<NoticeHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            context.Database.EnsureCreated();

            if(context.Stocks.Count() < 1)
            {
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 55, Change = 5, Symbol = "TVVT", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "TVVT", NoticeTime = DateTime.Now, EventType = "PRICEUP" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 55, Change = 5, Symbol = "MORT", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "MORT", NoticeTime = DateTime.Now, EventType = "PRICEUP" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 55, Change = 5, Symbol = "GOOG", updateTime = DateTime.Now});
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "GOOG", NoticeTime = DateTime.Now, EventType = "PRICEUP" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 55, Change = 5, Symbol = "MIST", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "MIST", NoticeTime = DateTime.Now, EventType = "PRICEUP" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 55, Change = 5, Symbol = "BOOK", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "BOOK", NoticeTime = DateTime.Now, EventType = "PRICEUP" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 45, Change = -5, Symbol = "CART", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "CART", NoticeTime = DateTime.Now, EventType = "PRICEDOWN" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 45, Change = -5, Symbol = "MART", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "MART", NoticeTime = DateTime.Now, EventType = "PRICEDOWN" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 45, Change = -5, Symbol = "BIKE", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "BIKE", NoticeTime = DateTime.Now, EventType = "PRICEDOWN" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 45, Change = -5, Symbol = "COOT", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "COOT", NoticeTime = DateTime.Now, EventType = "PRICEDOWN" });
                context.Stocks.Add(new Stock() { Price_pre = 50, Price_new = 45, Change = -5, Symbol = "ORAK", updateTime = DateTime.Now });
                context.Notice.Add(new Notice() { Id = Guid.NewGuid().ToString(), StockSymbol = "ORAK", NoticeTime = DateTime.Now, EventType = "PRICEDOWN" });

                context.SaveChanges();
            }

        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStock()
        {
            return await _context.Stocks.ToListAsync();
        }

        // GET: api/stock/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(string id)
        {
            var stock = await _context.Stocks.Where( s => s.Symbol == id).FirstOrDefaultAsync();

            if (stock == null)
            {
                return NotFound();
            }

            return stock;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(string id, Stock stock)
        {
            if (id != stock.Symbol)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;

            Notice notice = new Notice()
            {
                StockSymbol = stock.Symbol,
                EventType = "Edit"
            };
            _context.Notice.Add(notice);

            try
            {
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.PriceChangedEvent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stock
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _context.Stocks.Add(stock);

            Notice notice = new Notice()
            {
                StockSymbol = stock.Symbol,
                EventType = "Add"
            };
            _context.Notice.Add(notice);

            try
            {
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.PriceChangedEvent();
            }
            catch (DbUpdateException)
            {
                if (StockExists(stock.Symbol))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStock", new { id = stock.Symbol }, stock);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            Notice notification = new Notice()
            {
                StockSymbol = stock.Symbol,
                EventType = "Delete"
            };

            _context.Stocks.Remove(stock);
            _context.Notice.Add(notification);

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.PriceChangedEvent();

            return NoContent();
        }

        private bool StockExists(string id)
        {
            return _context.Stocks.Any(e => e.Symbol == id);
        }

        private async Task<List<Stock>> GetStockList()
        {
            return await _context.Stocks.ToListAsync();
        }

        [NonAction]
        public void UpdateClients()
        {
            this._hubContext.Clients.All.PriceChangedEvent();
        }

    }
}
