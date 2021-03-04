using FileReceiverBot.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot
{
    internal class TransactionsProcessor
    {
        public ITransactionProcessStrategy ProcessStrategy { get; set; }

        public void Process(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            ProcessStrategy?.ProcessTransaction(transaction, botClient, logger);
        }
    }
}
