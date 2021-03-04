using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class HelpCommand : IBotCommand
    {
        public string Name => "/help";

        public async void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, @"🔔 Чтобы проверить, включен ли я, используй команду /ison.
📨 Для отправки файла используй команду /send.");
            transaction.IsComplete = true;
        }
    }
}
