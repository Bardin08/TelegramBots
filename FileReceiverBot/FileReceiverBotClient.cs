using FileReceiverBot.Interfaces;
using Telegram.Bot;

namespace FileReceiverBot
{
    public class FileReceiverBotClient : IFileReceiverBotClient
    {
        public ITelegramBotClient BotClient
        {
            get;
        }

        public FileReceiverBotClient(string token)
        {
            BotClient = new TelegramBotClient(token);
        }
    }
}
