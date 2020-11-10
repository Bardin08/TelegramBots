using System.Collections.Generic;

namespace FileReceiverBot.Models
{
    public class CommandTransaction : TransactionBase
    {
        public CommandTransaction(int transactionId)
        {
            IsComplete = false;
            TransactionType = "CommandTransaction";
            MessageIds = new List<int>();
            TransactionId = RecepientId = transactionId;
        }
    }
}
