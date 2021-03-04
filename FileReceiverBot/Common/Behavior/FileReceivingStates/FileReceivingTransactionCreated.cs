using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceivingTransactionCreated : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as FileReceivingTransactionModel;
            logger.LogInformation("File sending transaction initialized by {username}({id})", currentTransaction.Username, currentTransaction.RecepientId);
            currentTransaction.TransactionState = new AskFileLabel();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }
    }
}
