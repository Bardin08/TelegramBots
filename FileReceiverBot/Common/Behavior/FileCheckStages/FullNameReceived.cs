using System;
using System.Threading.Tasks;

using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;

using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class FullNameReceived : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;
            if (currentTransaction.UserMessage.Text == null)
            {
                await HandleValidationError(transaction, botClient, logger);
                await MoveToPreviousState(transaction, botClient, logger, currentTransaction);
                return;
            }

            ProcessTransaction(logger, currentTransaction);
            await MoveToNextState(transaction, botClient, logger, currentTransaction);
        }

        private async Task HandleValidationError(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            MessageModel messageModel = new MessageModel()
            {
                Transaction = transaction,
                TextMessage = "Сообщение не распознано."
            };
            await TrySendMessage(messageModel, botClient, logger);
        }

        private static void ProcessTransaction(ILogger logger, FileSavedCheckTransactionModel currentTransaction)
        {
            currentTransaction.FullName = currentTransaction.UserMessage.Text;
            logger.LogDebug("User {username}({id}) real name received.", currentTransaction.Username, currentTransaction.RecepientId);
        }

        private static async Task MoveToNextState(object transaction, ITelegramBotClient botClient, ILogger logger, FileSavedCheckTransactionModel currentTransaction)
        {
            currentTransaction.TransactionState = new CheckFile();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }

        private static async Task MoveToPreviousState(object transaction, ITelegramBotClient botClient, ILogger logger, FileSavedCheckTransactionModel currentTransaction)
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
    }
}
