using FileReceiverBot.Common.Models;
using Telegram.Bot;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface IFileReceiverBotCommand
    {
        public string Name { get; }

        public void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient);
    }
}
