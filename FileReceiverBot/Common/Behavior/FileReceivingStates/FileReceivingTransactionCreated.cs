using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceivingTransactionCreated : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            transaction.TransactionState = new AskedFileLabel();
            transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
        }
    }
}
