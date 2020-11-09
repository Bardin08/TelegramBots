using System.Threading;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace FileReceiverBot
{
    public class FileReceiverBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<FileReceiverBot> _logger;

        public FileReceiverBot(ITelegramBotClient botClient, ILogger<FileReceiverBot> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogDebug("File Receiving Bot started");
            _botClient.OnMessage += MessageReceived;

            _botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private async void MessageReceived(object sender, MessageEventArgs e)
        {
            _logger.LogDebug("Received message from {0}", e.Message.From.Username);
            await _botClient.SendTextMessageAsync(e.Message.From.Id, "1");
        }
    }
}
