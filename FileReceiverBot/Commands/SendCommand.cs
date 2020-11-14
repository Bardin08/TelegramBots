using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class SendCommand : IFileReceiverBotCommand
    {
        public delegate void FileReceivingTransactionEvent(FileReceivingTransaction transaction);
        public static event FileReceivingTransactionEvent TransactionInitiated;

        public string Name => "/send";

        public void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient)
        {
            transaction.IsComplete = true;
            TransactionInitiated?.Invoke(new FileReceivingTransaction(transaction.RecepientId));
        }
    }
}
