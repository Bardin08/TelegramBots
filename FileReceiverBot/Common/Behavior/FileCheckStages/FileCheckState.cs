﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileReceiverBot.Common.Interfaces;
using FileReceiverBot.Common.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Common.Behavior.FileCheckStages
{
    internal class FileCheckState : IFileCheckTransactionState
    {
        public async Task ProcessTransactionAsync(Message message, FileCheckTransaction transaction, ITelegramBotClient botClient, ILogger logger)
        {
            var directoryInfo = new DirectoryInfo($"{BotConstants.FileSavingPath}{transaction.Label}\\{transaction.FullName}");

            if (!directoryInfo.Exists)
            {
                await botClient.SendTextMessageAsync(transaction.RecepientId,
                    directoryInfo.Exists
                        ? "Файл не был сохранен"
                        : "Проверь правильность отправленной информации, она должна точно совпадать с той, которую ты указывал при отправке файла!");
                transaction.IsComplete = true;
                return;
            }

            transaction.FilesInfo.AddRange(directoryInfo.GetFiles());

            var sb = new StringBuilder();

            sb.Append($"От {transaction.FullName} получен(-о) и сохранен(-о) {transaction.FilesInfo.Count} файл(-а/-ов).\n\n");

            transaction.FilesInfo.ForEach(f =>
            {
                sb.Append("\t").Append($"• {f.Name}({f.Length} bytes) сохранен в {f.CreationTime};\n");
            });

            await botClient.SendTextMessageAsync(transaction.RecepientId, sb.ToString());
            transaction.IsComplete = true;
        }
    }
}