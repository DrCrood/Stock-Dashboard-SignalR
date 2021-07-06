using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET5SignalR.Models
{
    public class Notice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        [Key]
        public string Id { get; set; }
        public string StockSymbol { get; set; }
        public DateTime NoticeTime { get; set; }
        public string EventType { get; set; }
    }
}
