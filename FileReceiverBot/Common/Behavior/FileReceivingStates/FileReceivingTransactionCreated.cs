using System.Collections.Generic;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using FileReceiverBot.FileReceivingStates;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class FileReceivingTransactionCreated : IFileReceivingTransactionState
    {
        public async void ProcessTransaction(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(transaction.RecepientId, "Привет. Ты успешно начал процесс отправки файла.");

            var buttons = new List<List<InlineKeyboardButton>>();

            var labels = new List<string> { "КДМ Лабораторная работа №3" };

            foreach (var label in labels)
            {
                var buttonsLine = new List<InlineKeyboardButton>();
                buttonsLine.Add(InlineKeyboardButton.WithCallbackData(label, label));
                buttons.Add(buttonsLine);
            }

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Выбери метку работы, которую хочешь сдать", replyMarkup: keyboard);

            transaction.MessageIds.Add(sentMessage.MessageId);
            transaction.TransactionState = new FileLabelReceived();
        }
    }
}
