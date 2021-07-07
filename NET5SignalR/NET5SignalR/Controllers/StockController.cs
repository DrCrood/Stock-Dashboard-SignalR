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
                context.Stocks.Add(new Stock("TVVT", 50, 55));
                context.Stocks.Add(new Stock("LESS", 50, 55));
                context.Stocks.Add(new Stock("GOOG", 50, 55));
                context.Stocks.Add(new Stock("MIST", 50, 55));
                context.Stocks.Add(new Stock("FACE", 50, 55));
                context.Stocks.Add(new Stock("GOOD", 50, 45));
                context.Stocks.Add(new Stock("HOSE", 50, 45));
                context.Stocks.Add(new Stock("TREE", 50, 45));
                context.Stocks.Add(new Stock("GMCC", 50, 45));
                context.Stocks.Add(new Stock("FORD", 50, 45));

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

            Notice notice = new Notice(stock.Symbol, stock.Change);
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

            Notice notice = new Notice(stock.Symbol, stock.Change);
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
        public async Task<IActionResult> DeleteStock(string id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            Notice notification = new Notice(stock.Symbol, stock.Change);

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

        [NonAction]
        public async Task SendChangeEvent(Notice notice)
        {
            await this._hubContext.Clients.All.ChangeNotice( notice);
        }

        [NonAction]
        public void AddStock(Stock stock)
        {
            _context.Stocks.Add(stock);
            _context.SaveChanges();
        }

    }
}
