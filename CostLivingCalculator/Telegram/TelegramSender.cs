using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using CostLivingCalculator.DataBase;

namespace CostLivingCalculator.Telegram
{
    internal class TelegramSender
    {
        protected async Task SendTextMessage(
            ITelegramBotClient botClient,
            long chatId,
            string messageText,
            IReplyMarkup replyMarkup,
            CancellationToken cancellationToken,
            String? type = null)
        {
            Message sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken); //parseMode: ParseMode.Html (для добавления html текста с форматированием)

            new DatabaseController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId, type);
        }

        protected async Task SendTextMessage(
            ITelegramBotClient botClient,
            long chatId,
            string messageText,
            CancellationToken cancellationToken,
            String? type = null)
        {
            Message sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cancellationToken);

            new DatabaseController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId, type);
        }

        protected async Task EditMessageText(
            ITelegramBotClient botClient,
            long chatId,
            int messageId,
            string messageText,
            InlineKeyboardMarkup replyMarkup,
            CancellationToken cancellationToken)
        {
            Message sendMessage = await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: messageText,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }

        protected async Task EditMessageText(
            ITelegramBotClient botClient,
            long chatId,
            int messageId,
            string messageText,
            CancellationToken cancellationToken)
        {
            Message sendMessage = await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: messageText,
                cancellationToken: cancellationToken);
        }
    }
}
