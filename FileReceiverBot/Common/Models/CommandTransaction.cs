using System.Collections.Generic;

namespace FileReceiverBot.Common.Models
{
    internal class CommandTransaction : TransactionBase
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
