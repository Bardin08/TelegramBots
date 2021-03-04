using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;

using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class AskLabel : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            InlineKeyboardMarkup keyboard = GenerateKeyboard();
            await TrySendMessage(transaction, keyboard, botClient, logger);
        }

        private InlineKeyboardMarkup GenerateKeyboard()
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
            return keyboard;
        }

        private static async Task TrySendMessage(object transaction, InlineKeyboardMarkup keyboard, ITelegramBotClient botClient, ILogger logger)
        {
            try
            {
                await SendMessage(transaction, keyboard, botClient);
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
        }

        private static async Task SendMessage(object transaction, InlineKeyboardMarkup keyboard, ITelegramBotClient botClient)
        {
            var currentTransaction = transaction as FileSavedCheckTransactionModel;
            var sentMessage = await botClient.SendTextMessageAsync(currentTransaction.RecepientId, "🔖Выбери метку работы, которую хочешь проверить", replyMarkup: keyboard);
            currentTransaction.MessageIds.Add(sentMessage.MessageId);
            currentTransaction.TransactionState = new FileLabelReceived();
        }

        private List<string> LoadFileLabels()
        {
            List<string> labels = new List<string>();

            using var reader = new StreamReader(BotConstants.LabelsFileFullName, System.Text.Encoding.Unicode);
            var line = "";

            while ((line = reader.ReadLine()) != null)
            {
                labels.Add(line.Split(';')[0]);
            }

            return labels;
        }
    }
}
