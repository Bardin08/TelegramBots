using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileTypeSelected : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (message.Text != null)
            {
                if (message.Text == "0")
                {
                    transaction.IsTeam = false;
                }
                else if (message.Text == "1")
                {
                    transaction.IsTeam = true;
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Ошибка распознования типа работы.");
                transaction.TransactionState = new FileTypeAsked();
                transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
                return;
            }

            transaction.TransactionState = new FullNameAsked();
            transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
        }
    }
}
