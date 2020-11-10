using FileReceiverBot.FileReceivingStates;
using FileReceiverBot.Interfaces;

namespace FileReceiverBot.Models
{
    public class FileReceivingTransaction : TransactionBase
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
    }
}
