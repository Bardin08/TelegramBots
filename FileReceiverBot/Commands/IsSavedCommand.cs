using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class IsSavedCommand : IFileReceiverBotCommand
    {
        public delegate void FileCheckTransactionEvent(FileCheckTransaction transaction);
        public static event FileCheckTransactionEvent FileCheckTransactionInitiated;

        public string Name => "/issaved";

        public void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient)
        {
            FileCheckTransactionInitiated?.Invoke(new FileCheckTransaction(message.From.Id));
        }
    }
}
