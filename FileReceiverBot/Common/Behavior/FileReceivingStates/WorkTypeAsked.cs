using System.Collections.Generic;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class WorkTypeAsked : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Личная👨‍🎓", "0") },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Командная👨‍🎓👩‍🎓", "1") }
            };

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Отлично, теперь выбери тип работы", replyMarkup: keyboard);

            transaction.MessageIds.Add(sentMessage.MessageId);
            transaction.TransactionState = new WorkTypeSelected();
        }
    }
}
