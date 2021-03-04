using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    public class FileCheckProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            (transaction as FileSavedCheckTransactionModel)?.TransactionState
                .ProcessAsync(transaction as FileSavedCheckTransactionModel, botClient, logger);
        }
    }
}
