using Telegram.Bot;

namespace FileReceiverBot.API.Services.Abstractions
{
    public interface IBotService
    {
        public ITelegramBotClient BotClient { get; set; }
    }
}
