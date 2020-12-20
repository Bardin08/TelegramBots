using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FullNameAsked : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, transaction.IsTeam ? "Отправь мне номер команды." : "Отправь мне своё ФИО.");

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
