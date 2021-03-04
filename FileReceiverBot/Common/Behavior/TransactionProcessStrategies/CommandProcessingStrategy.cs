using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FileReceiverBot.Common.Behavior.TransactionProcessStrategies
{
    internal class CommandProcessingStrategy : ITransactionProcessStrategy
    {
        public async void ProcessTransaction(object transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var commandTransaction = transaction as CommandTransactionModel;

            if (commandTransaction.UserMessage.Text == null)
            {
                await botClient.SendTextMessageAsync(commandTransaction.UserMessage.From.Id, "Ошибка распознования команды.");
                return;
            }

            var requiredCommand = LoadCommands()?.Find(c => c.Name == commandTransaction.UserMessage.Text);

            requiredCommand?.Execute(commandTransaction, botClient);
        }

        private List<IBotCommand> LoadCommands()
        {
            var commands = new List<IBotCommand>();
            var foundCommands = Assembly.GetExecutingAssembly().GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract
                && types.GetInterface("IBotCommand") != null).ToList();

            foreach (var command in foundCommands)
            {
                commands.Add((IBotCommand)Activator.CreateInstance(command));
            }

            return commands;
        }
    }
}
