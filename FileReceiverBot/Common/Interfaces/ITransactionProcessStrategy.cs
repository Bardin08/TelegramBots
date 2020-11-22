using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface ITransactionProcessStrategy
    {
        public void ProcessTransaction(Message message, object transaction, ITelegramBotClient botClient);
    }
}
