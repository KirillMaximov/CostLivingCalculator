using CostLivingCalculator.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CostLivingCalculator.Create
{
    internal class CalculatorBot
    {
        public TelegramBotClient BotClient;

        public CancellationTokenSource Cancellation;

        public CalculatorBot(String telegramToken)
        {
            BotClient = new TelegramBotClient(telegramToken);

            Cancellation = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            BotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: Cancellation.Token);
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var telegramController = new TelegramController();

                //Тип сообщения написанного боту в чате
                if (update.Type == UpdateType.Message)
                {
                    //Если тип сообщения текстовое
                    if (update.Message!.Type == MessageType.Text)
                    {
                        await telegramController.ResponsMessage(botClient, update, cancellationToken);
                    }
                }

                //Тип сообщения после выбора кнопки в меню
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await telegramController.ResponsCallbackQuery(botClient, update, cancellationToken);
                }
            }
            catch (Exception exp)
            {
                try
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery!.Id, $"Error: {exp.Message}", true);

                    var chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message!.Chat.Id;
                    Console.WriteLine($"В чате {chatId} произошла ошибка: {exp.ToString()}");
                }
                catch (Exception)
                {
                    var chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery!.Message!.Chat.Id;
                    Console.WriteLine($"В чате {chatId} произошла ошибка: {exp.ToString()}");
                }
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
