using Telegram.Bot;

namespace FileReceiverBot.Interfaces
{
    public interface IFileReceiverBotClient
    {
        ITelegramBotClient BotClient { get; }
    }
}
