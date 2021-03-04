using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    public class FileCheakTransactionInitiate : Interfaces.ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTransaction = transaction as Models.FileSavedCheckTransactionModel;
            currentTransaction.TransactionState = new AskLabel();
            await currentTransaction.TransactionState.ProcessAsync(transaction, botClient, logger);
        }
    }
}
