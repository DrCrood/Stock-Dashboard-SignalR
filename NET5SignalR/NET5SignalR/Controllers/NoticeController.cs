using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NET5SignalR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET5SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly StockDBContext _context;
        private readonly IHubContext<NoticeHub, IHubClient> _hubContext;

        public NoticeController(StockDBContext context, IHubContext<NoticeHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            context.Database.EnsureCreated();
        }

        // GET: api/Notice/all
        [Route("all")]
        [HttpGet]
        public async Task<ActionResult<List<Notice>>> GetAllNotices()
        {
            var notices = (from not in _context.Notice
                           select not);

            return await notices.ToListAsync();
        }

        // GET: api/Notice/count
        [Route("count")]
        [HttpGet]
        public async Task<ActionResult<int>> GetNoticeCount()
        {
            var c = _context.Notice.CountAsync();

            return await c;
        }

        // GET: api/Notice/id
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Notice>>> GetNotificationMessage(string id)
        {
            var results = from message in _context.Notice
                          where message.StockSymbol == id
                          select message;

            return await results.ToListAsync();
        }

    }
}
