using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot
{
    public class TransactionsProcessor
    {
        public ITransactionProcessStrategy ProcessStrategy { get; set; }

        public void Process(Message message, object transaction, ITelegramBotClient botClient)
        {
            ProcessStrategy.ProcessTransaction(message, transaction as FileReceivingTransaction, botClient);
        }
    }
}
