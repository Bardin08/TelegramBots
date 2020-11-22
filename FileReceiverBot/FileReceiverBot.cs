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
        private readonly ILogger<FileReceiverBot> _logger;

        private readonly TransactionsProcessor _transactionsProcessor;

        private List<object> _transactions;

        public FileReceiverBot(IFileReceiverBotClient botClient, ILogger<FileReceiverBot> logger)
        {
            _botClient = botClient.BotClient;
            _logger = logger;

            _transactions = new List<object>();
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
            _logger.LogInformation("New file receiving transaction initiated by user {id} at {timestamp}.",
                transaction.RecepientId,
                DateTime.Now.ToString());

            if (transaction != null)
            {
                _transactions.Add(transaction);
            }

            _transactionsProcessor.ProcessStrategy = new FileReceiptProcessingStrategy();
            _transactionsProcessor.Process(new Message(), transaction, _botClient);
        }

        private void CallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            _logger.LogDebug("Callback query received from {id} at {timestamt}.",
                e.CallbackQuery.Message.Chat.Id,
                DateTime.Now.ToString());

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
            _logger.LogDebug("Received message from {id} at {timestamp}.",
                message.From.Id,
                DateTime.Now.ToString());

            object userTransaction = _transactions.Find(transaction => (transaction as TransactionBase)?.RecepientId == message.From.Id);

            if (userTransaction != null)
            {
                _transactionsProcessor.ProcessStrategy = SelectStrategy(userTransaction as TransactionBase);
            }
            else if (message.Text?.StartsWith("/") == true)
            {
                _transactionsProcessor.ProcessStrategy = new CommandProcessingStrategy();
                userTransaction = new CommandTransaction(message.From.Id);
            }

            _transactionsProcessor.Process(message, userTransaction, _botClient);
        }

        private void DeleteCompleteTransactions()
        {
            var itemsRemove = _transactions.RemoveAll(x => (x as TransactionBase)?.IsComplete ?? false);

            _logger.LogInformation("Remove {n} complete transactions.", itemsRemove);
        }

        private ITransactionProcessStrategy SelectStrategy(TransactionBase transaction)
        {
            if (transaction.TransactionType == "FileReceiving")
            {
                return new FileReceiptProcessingStrategy();
            }

            return null;
        }
    }
}
