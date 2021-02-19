using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FullNameReceived : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            if (currentTransaction.UserMessage.Text == null)
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Сообщение не распознано.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User {username}({id}) sent incorrect file name. Message: {message}", transaction.Username, transaction.RecepientId, currentTransaction.UserMessage);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                transaction.TransactionState = new FullNameAsked();
                await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
                return;
            }

            transaction.SenderFullName = currentTransaction.UserMessage.Text;

            logger.LogDebug("User {username}({id}) real name received.", transaction.Username, transaction.RecepientId);
            transaction.TransactionState = new FileAsked();
            await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
        }
    }
}
