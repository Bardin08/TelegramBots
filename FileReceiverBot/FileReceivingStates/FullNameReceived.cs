using System.Reflection.Metadata.Ecma335;
using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FullNameReceived : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            if (message.Text == null)
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Сообщение не распознано.");

                transaction.TransactionState = new FullNameAsked();
                transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
                return;
            }

            transaction.SenderFullName = message.Text;

            transaction.TransactionState = new FileAsked();
            transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
        }
    }
}
