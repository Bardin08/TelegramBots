using System.Threading.Tasks;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface IFileCheckTransactionState
    {
        public Task ProcessTransactionAsync(Message message, FileCheckTransaction transaction,
            ITelegramBotClient botClient, ILogger logger);
    }
}
