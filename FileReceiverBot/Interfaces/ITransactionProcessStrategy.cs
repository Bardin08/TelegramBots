using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Interfaces
{
    public interface ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient);
    }
}
