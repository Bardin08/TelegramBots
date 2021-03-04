using FileReceiverBot.Common.Models;
using Telegram.Bot;

namespace FileReceiverBot.Abstractions
{
    internal interface IBotCommand
    {
        public string Name { get; }
        public void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient);
    }
}
