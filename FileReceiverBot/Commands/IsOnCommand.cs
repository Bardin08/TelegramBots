using FileReceiverBot.Common.Models;
using FileReceiverBot.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class IsOnCommand : IFileReceiverBotCommand
    {
        public string Name => "/ison";

        public async void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Бот включен");
        }
    }
}
