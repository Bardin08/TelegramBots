using System.Collections.Generic;
using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileLabelReceived : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
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
                transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
                return;
            }

            transaction.TransactionState = new FileTypeAsked();
            transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
        }
    }
}
