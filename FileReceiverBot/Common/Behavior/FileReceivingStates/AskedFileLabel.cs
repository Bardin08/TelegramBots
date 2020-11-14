using System.Collections.Generic;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using FileReceiverBot.FileReceivingStates;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    class AskedFileLabel : IFileReceivingTransactionState
    {
        public async void ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient)
        {
            var buttons = new List<List<InlineKeyboardButton>>();

            var labels = new List<string> { "КДМ Лабораторная работа №3" };

            foreach (var label in labels)
            {
                var buttonsLine = new List<InlineKeyboardButton>();
                buttonsLine.Add(InlineKeyboardButton.WithCallbackData(label, label));
                buttons.Add(buttonsLine);
            }

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Выбери метку работы, которую хочешь сдать", replyMarkup: keyboard).ConfigureAwait(false);

            transaction.MessageIds.Add(sentMessage.MessageId);
            transaction.TransactionState = new FileLabelReceived();
        }
    }
}
