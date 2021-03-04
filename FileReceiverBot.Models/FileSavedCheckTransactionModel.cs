using System.Collections.Generic;
using System.IO;
using FileReceiverBot.Common.Interfaces;

namespace FileReceiverBot.Common.Models
{
    internal class FileSavedCheckTransactionModel : BaseTransactionModel
    {
        public FileSavedCheckTransactionModel(int recepientId)
            : base (recepientId)
        {
            TransactionType = "FileCheck";
            FilesInfo = new List<FileInfo>();
            TransactionState = new Behavior.FileCheckStages.AskLabel();
        }

        public string Label { get; set; }
        public bool IsFileExists { get; set; }
        public string FullName { get; set; }
        public List<FileInfo> FilesInfo { get; set; }
        public ITransactionState TransactionState { get; set; }
    }
}
