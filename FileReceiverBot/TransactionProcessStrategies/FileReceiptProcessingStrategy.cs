using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.TransactionProcessStrategies
{
    public class FileReceiptProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, object transaction, ITelegramBotClient botClient)
        {

            (transaction as FileReceivingTransaction)?.TransactionState
                .ProcessTransaction(message, (FileReceivingTransaction)transaction, botClient);
        }
    }
}
