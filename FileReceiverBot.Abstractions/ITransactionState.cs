using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace FileReceiverBot.Common.Interfaces
{
    public interface ITransactionState
    {
        Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger);
    }
}
