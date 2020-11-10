using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.TransactionProcessStrategies
{
    public class FileReceiptProcessingStrategy : ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
        }
    }
}
