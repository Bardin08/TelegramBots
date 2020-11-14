using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface IFileReceiverBotCommand
    {
        public string Name { get; }

        public void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient);
    }
}
