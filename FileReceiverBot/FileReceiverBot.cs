using System;
using System.Collections.Generic;
using System.Threading;

using FileReceiverBot.Commands;
using FileReceiverBot.Common.Behavior.TransactionProcessStrategies;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace FileReceiverBot
{
    public class FileReceiverBot : IFileReceiverBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        private readonly TransactionsProcessor _transactionsProcessor;

        private readonly HashSet<object> _transactions;

        public FileReceiverBot(IFileReceiverBotClient botClient, ILogger<FileReceiverBot> logger)
        {
            _botClient = botClient.BotClient;
            _logger = logger;

            _transactions = new HashSet<object>();
            _transactionsProcessor = new TransactionsProcessor();
        }

        public void Execute()
        {
            _logger.LogDebug("File Receiving Bot started");

            _botClient.OnMessage += MessageReceived;
            _botClient.OnCallbackQuery += CallbackQueryReceived;

            SendCommand.TransactionInitiated += FileReceivingTransactionInitiated;
            IsSavedCommand.FileCheckTransactionInitiated += FileCheckTransactionInitiated;

            _botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private void FileCheckTransactionInitiated(FileSavedCheckTransactionModel transaction)
        {
            if (transaction != null)
            {
                _transactions.Add(transaction);
            }

            _transactionsProcessor.ProcessStrategy = new FileCheckProcessingStrategy();
            _transactionsProcessor.Process(transaction, _botClient, _logger);
        }

        private void FileReceivingTransactionInitiated(FileReceivingTransactionModel transaction)
        {
            if (transaction != null)
            {
                _transactions.Add(transaction);
            }

            _transactionsProcessor.ProcessStrategy = new FileReceivingStrategy();
            _transactionsProcessor.Process(transaction, _botClient, _logger);
        }

        private void CallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Message;
            message.Text = e.CallbackQuery.Data;
            message.From.Id = (int)message.Chat.Id;

            ProcessMessage(message);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            DeleteCompletedTransactions();

            ProcessMessage(e.Message);
        }

        private void ProcessMessage(Message message)
        {
            _transactions.TryGetValue(new BaseTransactionModel(message.From.Id), out object currentTransaction);
            var userTransaction = currentTransaction as BaseTransactionModel; 

            if (message.Text?.StartsWith("/") == true)
            {
                _transactions.RemoveWhere(t => ((BaseTransactionModel)t).RecepientId == message.From.Id);

                _transactionsProcessor.ProcessStrategy = new CommandProcessingStrategy();
                userTransaction = new CommandTransactionModel(message.From.Id);
            }
            else if (userTransaction != null)
            {
                _transactionsProcessor.ProcessStrategy = SelectTransactionProcessingStrategy(userTransaction);
            }

            if (userTransaction != null)
            {
                userTransaction.UserMessage = message;
            }

            _transactionsProcessor.Process(userTransaction, _botClient, _logger);
        }

        private void DeleteCompletedTransactions()
        {
            _transactions.RemoveWhere(t => ((BaseTransactionModel)t).IsComplete);
        }

        private ITransactionProcessStrategy SelectTransactionProcessingStrategy(BaseTransactionModel transaction)
        {
            if (transaction.TransactionType == "FileReceiving")
                return new FileReceivingStrategy();
            else if (transaction.TransactionType == "FileCheck")
                return new FileCheckProcessingStrategy();

            return null;    
        }
    }
}
