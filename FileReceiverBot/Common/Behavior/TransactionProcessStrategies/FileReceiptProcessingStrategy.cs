using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    internal class FileReceiptProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, object transaction, ITelegramBotClient botClient, ILogger logger)
        {

            (transaction as FileReceivingTransaction)?.TransactionState
                .ProcessTransactionAsync(message, (FileReceivingTransaction)transaction, botClient, logger);
        }
    }
}
