using System;

using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class AskFullName : IFileCheckTransactionState
    {
        public async Task ProcessTransactionAsync(FileSavedCheckTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Отправь мне своё ФИО или название команды.");

                if (sentMessage != null)
                {
                    logger.LogDebug("Sender credentials asked from {username}({id})", transaction.Username, transaction.RecepientId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }

            transaction.TransactionState = new FullNameReceived();
        }
    }
}
