using System.Collections.Generic;

namespace FileReceiverBot.Models
{
    public class TransactionBase
    {
        public TransactionBase()
        {
            MessageIds = new List<int>();
        }

        public bool IsComplete { get; set; }
        public int TransactionId { get; set; }
        public int RecepientId { get; set; }
        public string TransactionType { get; set; }
        public List<int> MessageIds { get; set; }
    }
}
