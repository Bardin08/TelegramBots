using FileReceiverBot.API.Services.Abstractions;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

namespace FileReceiverBot.API.Services.Implementations
{
    public class BotService : IBotService
    {
        public BotService(IConfiguration config)
        {
            BotClient = new TelegramBotClient(config.GetSection("Bot")["Token"]);
        }

        public ITelegramBotClient BotClient { get; set; }
    }
}
