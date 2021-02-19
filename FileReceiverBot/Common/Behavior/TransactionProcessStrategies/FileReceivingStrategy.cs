using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    internal class FileReceivingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(object transaction, ITelegramBotClient botClient, ILogger logger)
        {

            (transaction as FileReceivingTransactionModel)?.TransactionState
                .ProcessTransactionAsync((FileReceivingTransactionModel)transaction, botClient, logger);
        }
    }
}
