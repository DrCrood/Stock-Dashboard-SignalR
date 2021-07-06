using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET5SignalR.Models
{
    public class StockDBContext : DbContext
    {
        public StockDBContext(DbContextOptions options)
            : base(options)
        {
        }

        //need to be virtual for working with Moq
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Notice> Notice { get; set; }
    }
}
