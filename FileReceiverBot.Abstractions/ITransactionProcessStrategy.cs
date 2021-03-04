using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface ITransactionProcessStrategy
    {
        public void ProcessTransaction(object transaction, ITelegramBotClient botClient, ILogger logger);
    }
}
