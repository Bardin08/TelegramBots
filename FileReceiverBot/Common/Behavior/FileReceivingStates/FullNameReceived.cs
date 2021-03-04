using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FullNameReceived : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            if (currentTransaction.UserMessage.Text == null)
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "Сообщение не распознано.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User {username}({id}) sent incorrect file name. Message: {message}", currentTransaction.Username, currentTransaction.RecepientId, currentTransaction.UserMessage);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }

                currentTransaction.TransactionState = new FullNameAsked();
                await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
                return;
            }

            currentTransaction.SenderFullName = currentTransaction.UserMessage.Text;

            logger.LogDebug("User {username}({id}) real name received.", currentTransaction.Username, currentTransaction.RecepientId);
            currentTransaction.TransactionState = new FileAsked();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }
    }
}
