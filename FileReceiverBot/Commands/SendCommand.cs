using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using FileReceiverBot.Delegates;

using Telegram.Bot;

namespace FileReceiverBot.Commands
{
    internal class SendCommand : IBotCommand
    {
        public static event FileReceivingTransactionHandler TransactionInitiated;

        public string Name => "/send";

        public void Execute(CommandTransactionModel transaction, ITelegramBotClient botClient)
        {
            transaction.IsComplete = true;
            TransactionInitiated?.Invoke(CreateFileReceivingTranasctionModel(transaction));
        }

        private static FileReceivingTransactionModel CreateFileReceivingTranasctionModel(CommandTransactionModel commandTransaction)
        {
            return new FileReceivingTransactionModel(commandTransaction.RecepientId) { Username = commandTransaction.UserMessage.From.Username };
        }
    }
}
