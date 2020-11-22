using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            foreach (var label in LoadFileLabels())
            {
                var buttonsLine = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(label, label)
                };
                buttons.Add(buttonsLine);
            }

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "🔖Выбери метку работы, которую хочешь сдать", replyMarkup: keyboard);

            transaction.MessageIds.Add(sentMessage.MessageId);
            transaction.TransactionState = new FileLabelReceived();
        }

        private List<string> LoadFileLabels()
        {
            using (var reader = new StreamReader(BotConstants.LabelsFileFullName, System.Text.Encoding.Unicode))
            {
                var labes = reader.ReadToEnd().Split(',').ToList();
                labes.ForEach(l => l.Trim());

                return labes;
            }
        }
    }
}
