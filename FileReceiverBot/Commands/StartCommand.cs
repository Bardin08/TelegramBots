using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Commands
{
    public class StartCommand : IFileReceiverBotCommand
    {
        public string Name => "/start";

        public async void Execute(Message message, CommandTransaction transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync((transaction as CommandTransaction).RecepientId, "Этот бот предназначен для сохранения файлов. Для отправки файлов воспользуйся командой /send");
            (transaction as CommandTransaction).IsComplete = true;
        }
    }
}
