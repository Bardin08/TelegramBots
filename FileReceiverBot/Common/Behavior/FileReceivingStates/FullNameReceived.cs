using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FullNameReceived : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            if (message.Text == null)
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Сообщение не распознано.");

                transaction.TransactionState = new FullNameAsked();
                transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
                return;
            }

            transaction.SenderFullName = message.Text;

            transaction.TransactionState = new FileAsked();
            transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
        }
    }
}
