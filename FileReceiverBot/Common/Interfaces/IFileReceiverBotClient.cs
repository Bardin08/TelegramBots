using Telegram.Bot;

namespace FileReceiverBot.Common.Interfaces
{
    public interface IFileReceiverBotClient
    {
        ITelegramBotClient BotClient { get; }
    }
}
