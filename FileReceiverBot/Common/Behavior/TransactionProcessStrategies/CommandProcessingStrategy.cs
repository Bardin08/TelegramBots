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

            var commands = LoadCommands();

            var requiredCommand = commands?.Find(c => c.Name == commandTransaction.UserMessage.Text);

            requiredCommand?.Execute(commandTransaction, botClient);
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
