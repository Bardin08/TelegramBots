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
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Привет. Ты успешно начал процесс отправки файла.");

            transaction.TransactionState = new AskedFileLabel();
            transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient);
        }
    }
}
