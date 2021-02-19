using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class WorkTypeSelected : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            transaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(transaction.RecepientId, m));
            transaction.MessageIds.Clear();

            if (currentTransaction.UserMessage.Text != null)
            {
                if (currentTransaction.UserMessage.Text == "0")
                {
                    transaction.IsTeam = false;
                }
                else if (currentTransaction.UserMessage.Text == "1")
                {
                    transaction.IsTeam = true;
                }
            }
            else
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Ошибка распознования типа работы.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User {username}({id}) sent incorrect work type.", transaction.Username, transaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }


                transaction.TransactionState = new WorkTypeAsked();
                await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
                return;
            }

            transaction.TransactionState = new FullNameAsked();
            await transaction.TransactionState.ProcessTransactionAsync(transaction, botClient, logger);
        }
    }
}
