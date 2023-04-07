using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types;

namespace CostLivingCalculator.Models
{
    internal class UpdateModel
    {
        public long ChatId { get; }
        public int MessageId { get; }
        public string? MessageText { get; }
        public string? Username { get; }
        public string? FirstName { get; }
        public string? SecondName { get; }
        public string? ButtonData { get; }
        public SuccessfulPayment? Payment { get; }

        public UpdateModel(Update update)
        {
            if (update.Message != null)
            {
                ChatId = update.Message.Chat.Id;
                MessageText = update.Message.Text!;
                Username = update.Message.From!.Username!;
                FirstName = update.Message.From.FirstName;
                SecondName = update.Message.From.LastName!;
                Payment = update.Message.SuccessfulPayment!;
            }
            if (update.CallbackQuery != null)
            {
                ChatId = update.CallbackQuery.Message!.Chat.Id;
                MessageId = update.CallbackQuery.Message.MessageId;
                Username = update.CallbackQuery.From!.Username!;
                ButtonData = update.CallbackQuery.Data!;
            }
        }
    }
}
