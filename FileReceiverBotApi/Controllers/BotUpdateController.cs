using System.Threading.Tasks;

using FileReceiverBot.API.Services.Abstractions;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace FileReceiverBotApi.Controllers
{
    [Route("api/botupdate")]
    public class BotUpdateController : Controller
    {
        private readonly IBotService _bot;

        public BotUpdateController(IBotService bot)
        {
            _bot = bot;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update != null)
                await _bot.BotClient.SendTextMessageAsync(update.Message.From.Id, update.Message?.Text);

            return Ok();
        }
    }
}
