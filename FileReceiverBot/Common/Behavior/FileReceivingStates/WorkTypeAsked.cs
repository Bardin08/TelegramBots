using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class WorkTypeAsked : ITransactionState
    {
        public async Task ProcessAsync(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var currentTranasaction = transaction as FileReceivingTransactionModel;

            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Личная👨‍🎓", "0") },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Командная👨‍🎓👩‍🎓", "1") }
            };

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(currentTranasaction.RecepientId, "Отлично, теперь выбери тип работы", replyMarkup: keyboard);

                if (sentMessage != null)
                {
                    logger.LogDebug("The keyboard is sent to the {username}({id}) to select the type of work", currentTranasaction.Username, currentTranasaction.RecepientId);
                }

                currentTranasaction.MessageIds.Add(sentMessage.MessageId);
                currentTranasaction.TransactionState = new WorkTypeSelected();
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
        }
    }
}
