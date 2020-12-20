using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Behavior.FileReceivingStates
{
    internal class WorkTypeAsked : IFileReceivingTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileReceivingTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Личная👨‍🎓", "0") },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Командная👨‍🎓👩‍🎓", "1") }
            };

            var keyboard = new InlineKeyboardMarkup(buttons.ToArray());

            try
            {
                var sentMessage = await botClient.SendTextMessageAsync(transaction.RecepientId, "Отлично, теперь выбери тип работы", replyMarkup: keyboard);

                if (sentMessage != null)
                {
                    logger.LogDebug("The keyboard is sent to the {username}({id}) to select the type of work", transaction.Username, transaction.RecepientId);
                }

                transaction.MessageIds.Add(sentMessage.MessageId);
                transaction.TransactionState = new WorkTypeSelected();
            }
            catch (Exception ex)
            {
                logger.LogError("Message wasn`t sent. Error: {error}", ex.Message);
            }
        }
    }
}
