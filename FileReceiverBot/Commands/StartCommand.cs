using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    internal class StartCommand : IFileReceiverBotCommand
    {
        public string Name => "/start";

        public async void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, @"Привет! Я бот получения файлов студентов группы ІТ-01.
Через меня можно сдать свои работы.

❓Для получения дальнейшей информации нажми /help
✉️ Если у тебя есть предложения или пожелания - пиши @bardin_vlad");
            transaction.IsComplete = true;
        }
    }
}
