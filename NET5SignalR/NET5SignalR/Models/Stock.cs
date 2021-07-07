using System;
using System.ComponentModel.DataAnnotations;

namespace NET5SignalR.Models
{
    public class Stock
    {
        [Key]
        public string Symbol { get; set; }
        public int Price_pre { get; set; }
        public int Price_new { get; set; }
        public int Change { get; set; }
        public DateTime updateTime { get; set; }

        public Stock()
        {

        }
        public Stock(string sym, int pre, int newp)
        {
            this.Symbol = sym;
            this.Price_new = newp;
            this.Price_pre = pre;
            this.Change = this.Price_new - this.Price_pre;
            this.updateTime = DateTime.Now;
        }


    }
}
