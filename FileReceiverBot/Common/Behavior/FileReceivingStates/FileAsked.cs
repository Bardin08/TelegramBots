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
            logger.LogDebug("File request initialized for user {username}({id})", transaction.Username, transaction.RecepientId);

            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне файл, который нужно сохранить.");

                if (sentMessage != null)
                {
                    logger.LogDebug("File request sent to {username}({id})", transaction.Username, transaction.RecepientId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }

            transaction.TransactionState = new FileReceived();
        }
    }
}
