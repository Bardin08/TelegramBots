using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;

namespace FileReceiverBot.Commands
{
    internal class SendCommand : IFileReceiverBotCommand
    {
        public delegate void FileReceivingTransactionEvent(FileReceivingTransactionModel transaction);
        public static event FileReceivingTransactionEvent TransactionInitiated;

        public string Name => "/send";

        public void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            transaction.IsComplete = true;
            TransactionInitiated?.Invoke(new FileReceivingTransactionModel(transaction.RecepientId) { Username = transaction.UserMessage.From.Username });
        }
    }
}
