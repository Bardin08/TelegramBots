using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    public class FileAsked : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне файл, который нужно сохранить.");

            transaction.TransactionState = new FileReceived();
        }
    }
}
