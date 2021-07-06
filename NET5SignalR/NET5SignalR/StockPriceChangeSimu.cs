using System;
using System.Collections.Generic;
using NET5SignalR.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading;
using NET5SignalR.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

namespace NET5SignalR
{
    public class StockPriceChangeSimu : BackgroundService
    {
        private readonly StockDBContext _context;
        private readonly StockController _controller;
        private readonly Random rand;
        public StockPriceChangeSimu(IServiceProvider serviceProvider, IHubContext<NoticeHub, IHubClient> hubContext)
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder<StockDBContext>();
            builder.UseSqlite("Data Source=Stocks.db");

            DbContextOptions options = builder.Options;
            _context = new StockDBContext(builder.Options);

            this._controller = new StockController(_context, hubContext);

            rand = new Random();
        }
        public void Update()
        {
            var stockList = _context.Stocks.ToListAsync().GetAwaiter().GetResult();

            foreach(Stock s in stockList)
            {
                s.Price_pre = s.Price_new;
                int p = s.Price_new + ((int)(10 * (0.5 - rand.NextDouble())));
                s.Price_new = p > 2 ? p : 2;
                s.Change = s.Price_new - s.Price_pre;
            }

            _context.SaveChanges();

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simu background service started");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this.Update();
                _controller.UpdateClients();
                await Task.Delay(2000, stoppingToken);
            }

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simu background service stopped at: " + DateTime.Now.ToString());
            return base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
