using System;
using System.Collections.Generic;

using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Models
{
    internal class BaseTransactionModel
    {
        public BaseTransactionModel(int recepientId)
        {
            RecepientId = recepientId;
            TransactionId = Guid.NewGuid().ToString();

            MessageIds = new List<int>();
            IsComplete = false;
        }

        public string TransactionId { get; set; }
        public string Username { get; set; }
        public string TransactionType { get; set; }
        public int RecepientId { get; set; }
        public bool IsComplete { get; set; }
        public List<int> MessageIds { get; set; }
        public Message UserMessage { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (((BaseTransactionModel)obj).TransactionId == this.TransactionId) return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TransactionId.GetHashCode() * 31;
        }
    }
}
