using System;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class WorkTypeSelected : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            currentTransaction.MessageIds.ForEach(async m => await botClient.DeleteMessageAsync(currentTransaction.RecepientId, m));
            currentTransaction.MessageIds.Clear();

            if (currentTransaction.UserMessage.Text != null)
            {
                if (currentTransaction.UserMessage.Text == "0")
                {
                    currentTransaction.IsTeam = false;
                }
                else if (currentTransaction.UserMessage.Text == "1")
                {
                    currentTransaction.IsTeam = true;
                }
            }
            else
            {
                try
                {
                    var sentMessage = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "Ошибка распознования типа работы.");

                    if (sentMessage != null)
                    {
                        logger.LogDebug("User {username}({id}) sent incorrect work type.", currentTransaction.Username, currentTransaction.RecepientId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
                }


                currentTransaction.TransactionState = new WorkTypeAsked();
                await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
                return;
            }

            currentTransaction.TransactionState = new FullNameAsked();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }
    }
}
