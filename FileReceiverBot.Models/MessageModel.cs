using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiverBot.Common.Models
{
    public class MessageModel
    {
        public string TextMessage { get; set; }
        public InlineKeyboardMarkup Keyboard { get; set; }
        public object Transaction { get; set; }
    }
}
