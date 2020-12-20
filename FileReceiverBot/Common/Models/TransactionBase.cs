using System.Collections.Generic;

namespace FileReceiverBot.Common.Models
{
    internal class TransactionBase
    {
        public TransactionBase()
        {
            MessageIds = new List<int>();
        }

        public bool IsComplete { get; set; }
        public int TransactionId { get; set; }
        public int RecepientId { get; set; }
        public string Username { get; set; }
        public string TransactionType { get; set; }
        public List<int> MessageIds { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (((TransactionBase)obj).TransactionId == this.TransactionId) return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TransactionId.GetHashCode() * 31;
        }
    }
}
