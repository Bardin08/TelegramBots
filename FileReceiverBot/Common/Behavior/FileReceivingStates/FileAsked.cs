using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileAsked : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            try
            {
                _ = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "Отправь мне файл, который нужно сохранить.");
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }

            currentTransaction.TransactionState = new FileReceived();
        }
    }
}
