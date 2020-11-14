using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FullNameAsked : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            if (transaction.IsTeam)
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне номер команды.");
            }
            else
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне своё ФИО.");
            }

            transaction.TransactionState = new FullNameReceived();
        }
    }
}
