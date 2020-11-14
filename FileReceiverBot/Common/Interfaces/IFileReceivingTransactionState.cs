using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface IFileReceivingTransactionState
    {
        public void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient);
    }
}