using System;
using System.Collections.Generic;
using System.Threading;
using FileReceiverBot.Commands;
using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
using FileReceiverBot.TransactionProcessStrategies;
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
            if (transaction != null)
            {
                _transactions.Add(transaction);
            }

            _transactionsProcessor.ProcessStrategy = new FileReceiptProcessingStrategy();
            _transactionsProcessor.Process(new Message(), transaction, _botClient);
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
            DeleteCompleteTransactions();

            ProcessMessage(e.Message);
        }

        private void ProcessMessage(Message message)
        {
            object userTransaction = _transactions.Find(transaction => (transaction as TransactionBase)?.RecepientId == message.From.Id);

            if (message.Text?.StartsWith("/") == true)
            {
                if (userTransaction == null)
                {
                    _transactionsProcessor.ProcessStrategy = new CommandProcessingStrategy();
                    userTransaction = new CommandTransaction(message.From.Id);
                }
            }
            else if ((userTransaction as TransactionBase)?.TransactionType == "FileReceiving")
            {
                _transactionsProcessor.ProcessStrategy = new FileReceiptProcessingStrategy();
            }


            if (userTransaction != null)
            {
                _transactionsProcessor.Process(message, userTransaction, _botClient);
            }
        }

        private void DeleteCompleteTransactions()
        {
            _transactions.RemoveAll(x => (x as TransactionBase)?.IsComplete ?? false);
        }
    }
}
