using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileAsked : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            try
            {
                _ = await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне файл, который нужно сохранить.");
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }

            transaction.TransactionState = new FileReceived();
        }
    }
}
