using System;

using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class AskFullName : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            MessageModel messageModel = GenerateMessage(transaction);
            await TrySendMessage(messageModel, botClient, logger);
            MoveToNextState(transaction);
        }

        private static void MoveToNextState(object transaction)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;

            currentTransaction.TransactionState = new FullNameReceived();
        }

        private static MessageModel GenerateMessage(object transaction)
        {
            return new MessageModel()
            {
                Transaction = transaction,
                TextMessage = "Отправь мне своё ФИО или название команды."
            };
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
