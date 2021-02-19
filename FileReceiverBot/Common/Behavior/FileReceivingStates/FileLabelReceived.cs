using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileReceiverBot.Common.Behavior.FileReceivingStates;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileLabelReceived : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (currentTransaction.UserMessage.Text != null)
            {
                if (LoadFileLabels().Contains(currentTransaction.UserMessage.Text))
                {
                    transaction.FileInfo.Label = currentTransaction.UserMessage.Text;               

                    logger.LogInformation("File label: {label} received from {username}({id})", currentTransaction.UserMessage.Text, transaction.Username, transaction.RecepientId);
                    transaction.TransactionState = new WorkTypeAsked();
                    await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
                }
                else
                {
                    await botClient.SendTextMessageAsync(transaction.RecepientId, $"Метки *{currentTransaction.UserMessage.Text}* нет в списке доступных меток. Для выбора правильной метки используй кнопки!");
                    BackToLabelSelection(transaction, botClient, logger);
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

                BackToLabelSelection(transaction, botClient, logger);
            }
        }
        
        private async void BackToLabelSelection(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.TransactionState = new FileReceivingTransactionCreated();
            await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
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
