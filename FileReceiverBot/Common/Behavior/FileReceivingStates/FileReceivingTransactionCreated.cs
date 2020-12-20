using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceivingTransactionCreated : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            logger.LogInformation("File sending transaction initialized by {username}({id})", transaction.Username, transaction.RecepientId);
            transaction.TransactionState = new AskedFileLabel();
            await transaction.TransactionState.ProcessTransactionAsync(message, transaction, botClient, logger);
        }
    }
}
