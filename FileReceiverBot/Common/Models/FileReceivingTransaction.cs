﻿using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Behavior.FileReceivingStates;

namespace FileReceiverBot.Common.Models
{
    internal class FileReceivingTransaction : TransactionBase
    {
        public FileReceivingTransaction(int recepientId)
        {
            RecepientId = TransactionId = recepientId;
            TransactionType = "FileReceiving";
            IsTeam = false;
            TransactionState = new FileReceivingTransactionCreated();
            FileInfo = new ReceivedFileInfo();
        }

        public bool IsTeam { get; set; }
        public string SenderFullName { get; set; }
        public ReceivedFileInfo FileInfo { get; set; }
        public IFileReceivingTransactionState TransactionState { get; set; }

        public override string ToString()
        {
            return "File Receiving Transaction:\n\t" +
                $"User: {SenderFullName}({Username}({RecepientId}))\n\t" +
                $"Transaction state: {TransactionState.GetType().Name}\n\t" +
                $"Work type: {(IsTeam ? "Team" : "Personal")}\n\t" +
                $"File info: {FileInfo}";
        }
    }
}
