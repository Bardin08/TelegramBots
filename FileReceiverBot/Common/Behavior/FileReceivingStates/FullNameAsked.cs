using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FullNameAsked : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, currentTransaction.IsTeam ? "Отправь мне номер команды." : "Отправь мне своё ФИО.");

                if (sentMessage != null)
                {
                    logger.LogDebug("Sender credentials asked from {username}({id})", currentTransaction.Username, currentTransaction.RecepientId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }

            currentTransaction.TransactionState = new FullNameReceived();
        }
    }
}
