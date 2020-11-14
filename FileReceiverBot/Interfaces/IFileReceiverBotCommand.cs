using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Interfaces
{
    internal interface IFileReceiverBotCommand
    {
        public string Name { get; }

        public void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient);
    }
}
