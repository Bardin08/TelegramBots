using System;
using System.IO;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceived : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            if (message.Document == null)
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "❌Сообщение не содержит документа.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User ({username}({id}))", transaction.Username, transaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                transaction.TransactionState = new FileAsked();
                await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
                return;
            }

            transaction.FileInfo.Id = message.Document.FileId;
            transaction.FileInfo.Name = message.Document.FileName;

            logger.LogInformation("File {fileName} received from {username}.", transaction.FileInfo.Name, transaction.Username);

            var fileDirectory = $"C:\\Users\\User\\Desktop\\IT01.Telegram.Bots\\Received\\Files\\{transaction.FileInfo.Label}\\{transaction.SenderFullName}";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            var filePath = $"{fileDirectory}\\{transaction.FileInfo.Name}";

            var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            await botClient.GetInfoAndDownloadFileAsync(transaction.FileInfo.Id, fs);
            fs.Dispose();

            logger.LogInformation("File {fileName} saved to {filepath}", transaction.FileInfo.Name, filePath);

            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "✅Файл сохранен!");

                if (sentMessage != null)
                {
                    logger.LogDebug("User ({username}({id})) was sucessfully informed about file saving.", transaction.Username, transaction.RecepientId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
            transaction.IsComplete = true;
        }
    }
}
