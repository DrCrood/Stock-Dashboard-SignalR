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
    }
}
