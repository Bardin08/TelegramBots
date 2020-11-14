using FileReceiverBot.Common.Behavior.FileReceivingStates;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileLabelReceived : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (message.Text != null)
            {
                transaction.FileInfo.Label = message.Text;
            }
            else
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Ошибка распознования метки.");
                transaction.TransactionState = new FileReceivingTransactionCreated();
                transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
                return;
            }

            transaction.TransactionState = new FileTypeAsked();
            transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
        }
    }
}
