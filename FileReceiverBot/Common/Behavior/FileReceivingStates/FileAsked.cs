using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileAsked : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне файл, который нужно сохранить.");

            transaction.TransactionState = new FileReceived();
        }
    }
}
