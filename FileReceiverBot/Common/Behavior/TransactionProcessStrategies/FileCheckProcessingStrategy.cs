using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    public class FileCheckProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            (transaction as FileSavedCheckTransactionModel)?.TransactionState
                .ProcessTransactionAsync(transaction as FileSavedCheckTransactionModel, botClient, logger);
        }
    }
}
