using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class CheckFile : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;

            string path = GenerateDirectoryPath(currentTransaction);
            MessageModel messageModel = ProcessTransaction(transaction, logger, currentTransaction, path);

            await TrySendMessage(messageModel, botClient, logger);
            currentTransaction.IsComplete = true;
        }

        private MessageModel ProcessTransaction(object transaction, ILogger logger, FileSavedCheckTransactionModel currentTransaction, string path)
        {
            MessageModel messageModel = new MessageModel()
            {
                Transaction = transaction
            };

            if (!IsDirectoryExists(path))
            {
                messageModel.TextMessage = "Проверь правильность отправленной информации, она должна точно совпадать с той, которую ты указывал при отправке файла!";
            }
            else
            {
                currentTransaction.FilesInfo.AddRange(TryGetFiles(path, logger));
                messageModel.TextMessage = GenerateAnswer(currentTransaction);
            }

            return messageModel;
        }

        private bool IsDirectoryExists(string path)
        {
            return new DirectoryInfo(path).Exists;
        }

        private async Task TrySendMessage(MessageModel messageModel, ITelegramBotClient botClient, ILogger logger)
        {
            try
            {
                await SendMessage(messageModel, botClient);
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
        }

        private async Task SendMessage(MessageModel messageModel, ITelegramBotClient botClient)
        {
            var currentTransaction = messageModel.Transaction as FileSavedCheckTransactionModel;

            await botClient.SendTextMessageAsync(currentTransaction.RecepientId,
                messageModel.TextMessage,
                replyMarkup: messageModel.Keyboard);
        }

        private string GenerateDirectoryPath(FileSavedCheckTransactionModel currentTransaction)
        {
            return $"{BotConstants.FileSavingPath}{currentTransaction.Label}\\{currentTransaction.FullName}";
        }

        private FileInfo[] TryGetFiles(string path, ILogger logger)
        {
            try
            {
                return GetFiles(path);
            }
            catch (Exception ex)
            {
                logger.LogDebug($"Files not exist. Exception: {ex.Message}");
                return new FileInfo[0];
            }
        }

        private FileInfo[] GetFiles(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.GetFiles();
        }

        private static string GenerateAnswer(FileSavedCheckTransactionModel currentTransaction)
        {
            var sb = new StringBuilder();

            sb.Append("От ")
                .Append(currentTransaction.FullName)
                .Append(" получен(-о) и сохранен(-о) ")
                .Append(currentTransaction.FilesInfo.Count)
                .Append(" файл(-а/-ов).\n\n");

            currentTransaction.FilesInfo.ForEach(f =>
            {
                sb.Append("\t")
                  .Append("• ")
                  .Append(f.Name)
                  .Append('(')
                  .Append(f.Length)
                  .Append(" bytes) сохранен в ")
                  .Append(f.CreationTime).Append(";\n");
            });
            return sb.ToString();
        }
    }
}
