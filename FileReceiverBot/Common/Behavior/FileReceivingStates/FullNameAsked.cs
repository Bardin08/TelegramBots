using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
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
