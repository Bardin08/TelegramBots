using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using FileReceiverBot.FileReceivingStates;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    class AskedFileLabel : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            logger.LogDebug("File label request initialized for user {username}({id})", transaction.Username, transaction.RecepientId);

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

            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "🔖Выбери метку работы, которую хочешь сдать", replyMarkup: keyboard);

                transaction.MessageIds.Add(sentMessage.MessageId);
                transaction.TransactionState = new FileLabelReceived();

                if (sentMessage != null)
                {
                    logger.LogDebug("The file tag is requested from the user {username}({id})", transaction.Username, transaction.RecepientId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
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
