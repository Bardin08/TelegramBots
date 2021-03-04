using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileReceiverBot.Common.Exceptions;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceived : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            if (currentTransaction.UserMessage.Document == null)
            {
                try
                {
                    await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "❌Сообщение не содержит документа.");
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                currentTransaction.TransactionState = new FileAsked();
                await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
                return;
            }

            currentTransaction.FileInfo.Id = currentTransaction.UserMessage.Document.FileId;
            currentTransaction.FileInfo.Name = currentTransaction.UserMessage.Document.FileName;

            logger.LogInformation("File {fileName} received from {username}.", currentTransaction.FileInfo.Name, currentTransaction.Username);

            try
            {
                var fileCheckResult = CheckFileName(currentTransaction.FileInfo.Name, currentTransaction.FileInfo.Label);

                if (!fileCheckResult.CanFileBeSave)
                {
                    var sb = new StringBuilder();

                    sb.Append("В названии файла найдены ошибки:\n");

                    fileCheckResult.Errors.ForEach(e => sb.Append("• ").Append(e).Append(";").Append("\n"));

                    await botClient.SendTextMessageAsync(currentTransaction.RecepientId, sb.ToString());
                    await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "Переименуй файл и начни отправку заново");

                    currentTransaction.IsComplete = true;
                }
                else
                {
                    SaveFile(currentTransaction, botClient, logger);
                }
            }
            catch (InternalBotErrorException ex)
            {
                await botClient.SendTextMessageAsync(currentTransaction.RecepientId, ex.Message);
                currentTransaction.IsComplete = true;
            }
        }

        private async void SaveFile(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
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

        private (bool CanFileBeSave, List<string> Errors) CheckFileName(string name, string label)
        {
            string line = "";

            using (var reader = new StreamReader(BotConstants.LabelsFileFullName, Encoding.Unicode))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Split(';')[0].Equals(label))
                    {
                        break;
                    }
                }
            }

            bool canBeSaved = true;
            List<string> errors = new List<string>();

            if (line != null)
            {
                var pl = ParseLine(line);

                foreach (var np in pl.NameParts)
                {
                    if (!name.Contains(np))
                    {
                        canBeSaved = false;
                        errors.Add($"Имя файла не содержит части названия *{np}*");
                    }
                }

                if (!pl.Extensions.Contains(name[(name.IndexOf('.') + 1)..]))
                {
                    var sb = new StringBuilder();

                    pl.Extensions.ForEach(e => sb.Append($"{e}, "));

                    canBeSaved = false;
                    errors.Add($"Файл сохранен в неправильном расширении. Доступные расширения: {sb.ToString().Trim().TrimEnd(',')}; текущее расширение: {name[(name.IndexOf('.') + 1)..]}");
                }

                if (!canBeSaved)
                {
                    errors.Add($"Шаблон названия файла: {pl.Pattern}");
                }

            }
            else
            {
                throw new InternalBotErrorException("Произошла ошибка. Попробуй повторить попытку через несколько минут");
            }

            return (canBeSaved, errors);
        }

        private (string Label, List<string> NameParts, List<string> Extensions, string Pattern) ParseLine(string line)
        {
            var devidenLine = line.Split(';');

            var nameParts = new List<string>();
            var extensions = new List<string>();

            devidenLine[1].Split(',').ToList().ForEach(np => nameParts.Add(np.Trim()));
            devidenLine[2].Split(',').ToList().ForEach(ext => extensions.Add(ext.Trim()));

            return (devidenLine[0].Trim(), nameParts, extensions, devidenLine[3].Trim());
        }
    }
}
