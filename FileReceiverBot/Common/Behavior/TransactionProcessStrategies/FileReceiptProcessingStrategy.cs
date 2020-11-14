using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    internal class FileReceiptProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, object transaction, ITelegramBotClient botClient)
        {

            (transaction as FileReceivingTransaction)?.TransactionState
                .ProcessTransaction(message, (FileReceivingTransaction)transaction, botClient);
        }
    }
}
