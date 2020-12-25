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

            _botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private void FileReceivingTransactionInitiated(FileReceivingTransaction transaction)
        {
            _logger.LogDebug("New file receiving transaction initiated by {username}({id}).\n{transaction}",
                transaction.Username, transaction.RecepientId, transaction);

            if (transaction != null)
            {
                _transactions.Add(transaction);
            }

            _transactionsProcessor.ProcessStrategy = new FileReceivingStrategy();
            _transactionsProcessor.Process(new Message(), transaction, _botClient, _logger);
        }

        private void CallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            _logger.LogDebug("Received callback query: {callbackQuery}. \n", e.CallbackQuery);

            var message = e.CallbackQuery.Message;
            message.Text = e.CallbackQuery.Data;
            message.From.Id = (int)message.Chat.Id;

            ProcessMessage(message);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            DeleteCompleteTransactions();

            ProcessMessage(e.Message);
        }

        private void ProcessMessage(Message message)
        {
            _logger.LogDebug($"{message}\n");

            _transactions.TryGetValue(new TransactionBase() { TransactionId = message.From.Id }, out object userTransaction);

            if (message.Text?.StartsWith("/") == true)
            {
                _transactions.RemoveWhere(t => ((TransactionBase)t).TransactionId == message.From.Id);

                _transactionsProcessor.ProcessStrategy = new CommandProcessingStrategy();
                userTransaction = new CommandTransaction(message.From.Id);
            }
            else if (userTransaction != null)
            {
                _transactionsProcessor.ProcessStrategy = SelectTransactionProcessingStrategy(userTransaction as TransactionBase);
            }

            _transactionsProcessor.Process(message, userTransaction, _botClient, _logger);
        }

        private void DeleteCompleteTransactions()
        {
            _transactions.RemoveWhere(t => ((TransactionBase)t).IsComplete);
        }

        private ITransactionProcessStrategy SelectTransactionProcessingStrategy(TransactionBase transaction)
        {
            if (transaction.TransactionType == "FileReceiving")
            {
                return new FileReceivingStrategy();
            }

            return null;
        }
    }
}
