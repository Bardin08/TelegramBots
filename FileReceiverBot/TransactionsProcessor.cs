using FileReceiverBot.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot
{
    internal class TransactionsProcessor
    {
        public ITransactionProcessStrategy ProcessStrategy { get; set; }

        public void Process(Message message, object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            ProcessStrategy?.ProcessTransaction(message, transaction, botClient, logger);
        }
    }
}
