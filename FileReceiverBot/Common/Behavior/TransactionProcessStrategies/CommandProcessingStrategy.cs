using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    internal class CommandProcessingStrategy : ITransactionProcessStrategy
    {
        public async void ProcessTransaction(Message message, object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            if (message.Text == null)
            {
                await botClient.SendTextMessageAsync(message.From.Id, "Ошибка распознования команды.");
                return;
            }

            var commands = LoadCommands();

            var requiredCommand = commands?.Find(c => c.Name == message.Text);

            requiredCommand?.Execute(message, transaction as CommandTransaction, botClient);
        }

        private List<IFileReceiverBotCommand> LoadCommands()
        {
            var commands = new List<IFileReceiverBotCommand>();
            var foundCommands = Assembly.GetExecutingAssembly().GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract
                && types.GetInterface("IFileReceiverBotCommand") != null).ToList();

            foreach (var command in foundCommands)
            {
                commands.Add((IFileReceiverBotCommand)Activator.CreateInstance(command));
            }

            return commands;
        }
    }
}
