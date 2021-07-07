using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NET5SignalR.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChangeType
    {
        PriceUp,
        PriceDn,
        Add,
        Remove,
        Default
    }
    public class Notice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        [Key]
        public string Id { get; set; }
        public string StockSymbol { get; set; }
        public DateTime NoticeTime { get; set; }
        public ChangeType EventType { get; set; }
    
        public Notice()
        {

        }

        public Notice(string sym, int change)
        {
            this.Id = Guid.NewGuid().ToString();
            this.StockSymbol = sym;
            this.NoticeTime = DateTime.Now;
            if(change > 0)
            {
                this.EventType = ChangeType.PriceUp;
            } else if(change < 0)
            {
                this.EventType = ChangeType.PriceDn;
            }
            else
            {
                this.EventType = ChangeType.Default;
            }
        }

    }

}
