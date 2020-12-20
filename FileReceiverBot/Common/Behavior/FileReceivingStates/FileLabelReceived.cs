using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Behavior.FileReceivingStates;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.FileReceivingStates
{
    internal class FileLabelReceived : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (message.Text != null)
            {
                transaction.FileInfo.Label = message.Text;
            }
            else
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "⚠️Ошибка распознования метки.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User ({username}({id})) sent a wrong file label.", transaction.Username, transaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                transaction.TransactionState = new FileReceivingTransactionCreated();
                await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
                return;
            }

            logger.LogInformation("File label: {label} received from {username}({id})", message.Text, transaction.Username, transaction.RecepientId);
            transaction.TransactionState = new WorkTypeAsked();
            await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
        }
    }
}
