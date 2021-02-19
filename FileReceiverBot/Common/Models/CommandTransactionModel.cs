namespace FileReceiverBot.Common.Models
{
    internal class CommandTransactionModel : BaseTransactionModel
    {
        public CommandTransactionModel(int recepientId)
            : base(recepientId)
        {
            TransactionType = "CommandTransaction";
        }
    }
}
