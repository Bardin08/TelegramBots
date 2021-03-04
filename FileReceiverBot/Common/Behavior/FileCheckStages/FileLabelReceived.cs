using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;

using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class FileLabelReceived : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;
            DeletePreviousMessages(botClient, currentTransaction);
            var validationResult = ValidateUserMessage(currentTransaction.UserMessage.Text);

            if (validationResult.IsValid)
            {
                Process(currentTransaction, logger);
                await MoveToNextState(transaction, botClient, logger, currentTransaction);
            }
            else
            {
                await HandleValidationError(transaction, botClient, logger, validationResult);
                MoveToPreviousState(currentTransaction, botClient, logger);
            }
        }

        private static void Process(FileSavedCheckTransactionModel currentTransaction, ILogger logger)
        {
            currentTransaction.Label = currentTransaction.UserMessage.Text;
            logger.LogInformation("File label: {label} received from {username}({id})",
                                  currentTransaction.UserMessage.Text,
                                  currentTransaction.Username,
                                  currentTransaction.RecepientId);

        }

        private async Task HandleValidationError(object transaction, ITelegramBotClient botClient, ILogger logger, (bool IsValid, List<string> Errors) validationResult)
        {
            MessageModel messageModel = new MessageModel()
            {
                Transaction = transaction,
                TextMessage = GenerateErrorMessage(validationResult)
            };

            await TrySendMessage(messageModel, botClient, logger);
        }

        private static async Task MoveToNextState(object transaction, ITelegramBotClient botClient, ILogger logger, FileSavedCheckTransactionModel currentTransaction)
        {
            currentTransaction.TransactionState = new AskFullName();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
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

        private string GenerateErrorMessage((bool IsValid, List<string> Errors) validationResult)
        {
            var sb = new StringBuilder();

            sb.Append("В сообщении найдена одна или нескольно ошибок.");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Список ошибок:");
            sb.AppendLine();
            
            foreach (var e in validationResult.Errors)
            {
                sb.Append("• ").Append(e).AppendLine();
            }

            return sb.ToString();
        }

        private static (bool IsValid, List<string> Errors) ValidateUserMessage(string? messageText)
        {
            bool isValid = true;
            List<string> errors = new List<string>();

            if (messageText == null)
            {
                isValid = false;
                errors.Add("⚠️Ошибка распознования метки.");
                return (isValid, errors);
            }

            if (!LoadFileLabels().Contains(messageText))
            {
                isValid = false;
                errors.Add($"Метки *{messageText}*нет в списке доступных меток. Для выбора правильной метки используй кнопки!");
            }

            return (isValid, errors);
        }

        private static void DeletePreviousMessages(ITelegramBotClient botClient, FileSavedCheckTransactionModel currentTransaction)
        {
            currentTransaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(currentTransaction.RecepientId, m));
            currentTransaction.MessageIds.Clear();
        }

        private async void MoveToPreviousState(FileSavedCheckTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.TransactionState = new AskLabel();
            await transaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }

        private static List<string> LoadFileLabels()
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
