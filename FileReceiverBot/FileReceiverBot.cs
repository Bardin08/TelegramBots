using System.Collections.Generic;
using System.Threading;
using FileReceiverBot.Interfaces;
using FileReceiverBot.Models;
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

        private List<object> _transactions;

        public FileReceiverBot(IFileReceiverBotClient botClient, ILogger<FileReceiverBot> logger)
        {
            _botClient = botClient.BotClient;
            _logger = logger;

            _transactions = new List<object>();
        }

        public void Execute()
        {
            _logger.LogDebug("File Receiving Bot started");

            _botClient.OnMessage += MessageReceived;
            _botClient.OnCallbackQuery += CallbackQueryReceived;

            _botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
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

            if (message.Text == "/send")
            {
                if (userTransaction == null)
                {
                    userTransaction = new FileReceivingTransaction(message.From.Id);
                    _transactions.Add(userTransaction);
                }
            }

            if ((userTransaction as TransactionBase)?.TransactionType == "FileReceiving")
            {
                var tp = new TransactionsProcessor
                {
                    ProcessStrategy = new TransactionProcessStrategies.FileReceiptProcessingStrategy()
                };

                tp.Process(message, userTransaction, _botClient);
            }
        }

        private void DeleteCompleteTransactions()
        {
            _transactions.RemoveAll(x => (x as TransactionBase).IsComplete);
        }
    }
}
