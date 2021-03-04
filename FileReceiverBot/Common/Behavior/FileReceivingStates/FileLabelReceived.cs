using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileReceiverBot.Common.Behavior.FileReceivingStates;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileLabelReceived : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            currentTransaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(currentTransaction.RecepientId, m));
            currentTransaction.MessageIds.Clear();

            if (currentTransaction.UserMessage.Text != null)
            {
                if (LoadFileLabels().Contains(currentTransaction.UserMessage.Text))
                {
                    currentTransaction.FileInfo.Label = currentTransaction.UserMessage.Text;               

                    logger.LogInformation("File label: {label} received from {username}({id})", currentTransaction.UserMessage.Text, currentTransaction.Username, currentTransaction.RecepientId);
                    currentTransaction.TransactionState = new WorkTypeAsked();
                    await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
                }
                else
                {
                    await botClient.SendTextMessageAsync(currentTransaction.RecepientId, $"Метки *{currentTransaction.UserMessage.Text}* нет в списке доступных меток. Для выбора правильной метки используй кнопки!");
                    BackToLabelSelection(currentTransaction, botClient, logger);
                }
            }
            else
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "⚠️Ошибка распознования метки.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User ({username}({id})) sent a wrong file label.", currentTransaction.Username, currentTransaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                BackToLabelSelection(currentTransaction, botClient, logger);
            }
        }
        
        private async void BackToLabelSelection(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.TransactionState = new FileReceivingTransactionCreated();
            await transaction.TransactionState.ProcessAsync(transaction, botClient, logger);
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
