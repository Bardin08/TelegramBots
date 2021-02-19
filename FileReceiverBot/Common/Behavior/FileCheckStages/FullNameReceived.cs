using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class FullNameReceived : IFileCheckTransactionState
    {
        public async Task ProcessTransactionAsync(FileSavedCheckTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;
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

                transaction.TransactionState = new AskFullName();
                await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
                return;
            }

            transaction.FullName = currentTransaction.UserMessage.Text;

            logger.LogDebug("User {username}({id}) real name received.", transaction.Username, transaction.RecepientId);
            transaction.TransactionState = new FileCheckState();
            await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
        }
    }
}
