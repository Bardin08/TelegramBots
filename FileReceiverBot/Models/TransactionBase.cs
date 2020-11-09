using System.Collections.Generic;

namespace FileReceiverBot.Models
{
    public class TransactionBase
    {
        public int TransactionId { get; set; }
        public int RecepientId { get; set; }
        public string TransactionType { get; set; }
        public List<int> MessageIds { get; set; }
    }
}
