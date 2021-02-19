using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;

namespace FileReceiverBot.Commands
{
    internal class IsSavedCommand : IFileReceiverBotCommand
    {
        public delegate void FileCheckTransactionEvent(FileSavedCheckTransactionModel transaction);
        public static event FileCheckTransactionEvent FileCheckTransactionInitiated;

        public string Name => "/issaved";

        public void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            FileCheckTransactionInitiated?.Invoke(
                new FileSavedCheckTransactionModel((transaction as CommandTransactionModel).UserMessage.From.Id));
        }
    }
}
