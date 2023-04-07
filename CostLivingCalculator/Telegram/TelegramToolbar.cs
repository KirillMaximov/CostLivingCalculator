using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CostLivingCalculator.Telegram
{
    internal class TelegramToolbar
    {
        private List<InlineKeyboardButton[]> keyboardButtons;

        public InlineKeyboardMarkup? keyboardMarkup;

        public TelegramToolbar()
        {
            keyboardButtons = new List<InlineKeyboardButton[]>();
        }

        public void AddInlineKeyboardButton(String text, String callbackData)
        {
            keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: text, callbackData: callbackData) });
            keyboardMarkup = new InlineKeyboardMarkup(keyboardButtons);
        }

        //public InlineKeyboardMarkup GetInlineKeyboardMarkup()
        //{
        //    return new InlineKeyboardMarkup(keyboardButtons);
        //}
    }
}
