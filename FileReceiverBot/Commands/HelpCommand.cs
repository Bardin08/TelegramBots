using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class HelpCommand : IFileReceiverBotCommand
    {
        public string Name => "/help";

        public async void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Help text here!");
            transaction.IsComplete = true;
        }
    }
}
