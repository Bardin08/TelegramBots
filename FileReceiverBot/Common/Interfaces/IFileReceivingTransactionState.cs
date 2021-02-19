using System.Threading.Tasks;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Interfaces
{
    internal interface IFileReceivingTransactionState
    {
        public Task ProcessTransactionAsync(FileReceivingTransactionModel transaction, ITelegramBotClient botClient, ILogger logger);
    }
}