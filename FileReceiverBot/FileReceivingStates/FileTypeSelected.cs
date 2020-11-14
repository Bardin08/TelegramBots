using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileTypeSelected : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
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
                transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
                return;
            }

            transaction.TransactionState = new FullNameAsked();
            transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
        }
    }
}
