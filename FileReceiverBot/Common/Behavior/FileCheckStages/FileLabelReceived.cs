using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class FileLabelReceived : IFileCheckTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileCheckTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (message.Text != null)
            {
                if (LoadFileLabels().Contains(message.Text))
                {
                    transaction.Label = message.Text;

                    logger.LogInformation("File label: {label} received from {username}({id})", message.Text, transaction.Username, transaction.RecepientId);
                    
                    transaction.TransactionState = new AskFullName();
                    await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
                }
                else
                {
                    await botClient.SendTextMessageAsync(transaction.RecepientId, $"Метки *{message.Text}* нет в списке доступных меток. Для выбора правильной метки используй кнопки!");
                    BackToLabelSelection(message, transaction, botClient, logger);
                }
            }
            else
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "⚠️Ошибка распознования метки.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User ({username}({id})) sent a wrong file label.", transaction.Username, transaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                BackToLabelSelection(message, transaction, botClient, logger);
            }
        }

        private async void BackToLabelSelection(Message message, FileCheckTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.TransactionState = new AskLabel();
            await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
        }

        private List<string> LoadFileLabels()
        {
            List<string> labels = new List<string>();

            using (var reader = new StreamReader(BotConstants.LabelsFileFullName, System.Text.Encoding.Unicode))
            {
                var line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    labels.Add(line.Split(';')[0]);
                }

                return labels;
            }
        }
    }
}
