using System.IO;
using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    public class FileReceived : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            if (message.Document == null)
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId, "Сообщение не содержит документа.");

                transaction.TransactionState = new FileAsked();
                transaction.TransactionState.ProcessTransaction(message, transaction, botClient);
                return;
            }

            transaction.FileInfo.Id = message.Document.FileId;
            transaction.FileInfo.Name = message.Document.FileName;

            var fileDirectory = $"C:\\Users\\User\\Desktop\\IT01.Telegram.Bots\\Received\\Files\\{transaction.FileInfo.Label}\\{transaction.SenderFullName}";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            var fs = new FileStream(fileDirectory + "\\" + transaction.FileInfo.Name, FileMode.OpenOrCreate);
            await botClient.GetInfoAndDownloadFileAsync(transaction.FileInfo.Id, fs);
            fs.Dispose();

            await botClient.SendTextMessageAsync(transaction.RecepientId, "Файл сохранен!");
            transaction.IsComplete = true;
        }
    }
}
