using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using CostLivingCalculator.Models;
using CostLivingCalculator.DataAccess;
using CostLivingCalculator.DataBase;
using System.Drawing;
using Azure;

namespace CostLivingCalculator.Telegram
{
    internal class TelegramController : TelegramSender
    {
        public async Task ResponsMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = new UpdateModel(update);

            Console.WriteLine($"Получено сообщение: '{message.MessageText}' в чате {message.ChatId} от {message.Username} ({message.FirstName} {message.SecondName})");

            if (message.MessageText == "/test")
            {
                #region Testing

                var qwe1 = GetMonth(1);
                var qwe2 = GetMonth(2);
                var qwe3 = GetMonth(3);

                #endregion

                return;
            }

            if (message.MessageText == "/start")
            {
                new DatabaseController().ClearDataBase(message.ChatId);

                await SendTextMessage(botClient, message.ChatId,
                    "Введите название вашего региона/области.",
                    cancellationToken, "RegionName");
                
                return;
            }

            var lastMessage = new DatabaseController().GetLastMessage(message.ChatId);

            if (lastMessage != null)
            {
                if (lastMessage.Type == "RegionName")
                {
                    var enableRegions = CheckRegion(message.MessageText ?? String.Empty);

                    if (enableRegions.Count > 0)
                    {
                        if (enableRegions.Count > 10)
                        {
                            await SendTextMessage(botClient, message.ChatId,
                                "Регион не найден, проверьте корректность написания (пример Московская область, Тульская область, Карелия).",
                                cancellationToken, "RegionName");
                        }
                        else
                        {
                            var toolbar = new TelegramToolbar();

                            foreach (var region in enableRegions)
                            {
                                toolbar.AddInlineKeyboardButton(region, region);
                            }

                            var markup = toolbar.keyboardMarkup;

                            await SendTextMessage(botClient, message.ChatId,
                                "Выберите регион. Если его нет в списке, то введите корректное название.",
                                markup!, cancellationToken, "RegionName");
                        }
                    }
                    else
                    {
                        await SendTextMessage(botClient, message.ChatId,
                            "Регион не найден, проверьте корректность написания (пример Московская область, Тульская область, Карелия).",
                            cancellationToken, "RegionName");
                    }
                }

                if (lastMessage.Type == "NumberPeople")
                {
                    var numberPeople = Convert.ToInt32(message.MessageText!.Trim());

                    //Записываем количество человек в БД
                    new DatabaseController().InsertAnswer(message.ChatId, numberPeople: numberPeople);

                    if (numberPeople > 0 && numberPeople < 11)
                    {
                        var toolbar = new TelegramToolbar();

                        toolbar.AddInlineKeyboardButton($"Трудоспособный человек", "Worker");
                        toolbar.AddInlineKeyboardButton($"Ребенок младше 18 лет", "Child");
                        toolbar.AddInlineKeyboardButton($"Пенсионер", "Pensioner");

                        var markup = toolbar.keyboardMarkup;

                        await SendTextMessage(botClient, message.ChatId,
                            $"Далее, необходимо указать к какой категории относятся все проживающие.",
                            cancellationToken, "CategoryPeople");
                        await SendTextMessage(botClient, message.ChatId,
                            $"Укажите к какой категории относится человек №1",
                            markup!, cancellationToken);
                    }
                    else
                    {
                        await SendTextMessage(botClient, message.ChatId,
                            $"Количество человек должно быть от 1 до 10",
                            cancellationToken);
                    }
                }

                if (lastMessage.Type == "MonthlyIncome")
                {
                    var dbController = new DatabaseController();

                    try
                    {
                        var amount = Convert.ToInt32(message.MessageText!.Trim());

                        dbController.InsertMonthlyIncome(message.ChatId, amount);

                        var countIncome = dbController.GetAddedIncome(message.ChatId);

                        if (countIncome < 3)
                        {
                            await SendTextMessage(botClient, message.ChatId,
                                $"Укажите доход за {GetMonth(countIncome + 1)} (пример: 34688)",
                                cancellationToken);
                        }
                        else
                        {
                            var averageIncome = GetAverageIncome(message.ChatId); //(14+17+22/4)
                            var livingWage = GetLivingWage(message.ChatId); //(15+15+12+12/4=13.5)

                            await SendTextMessage(botClient, message.ChatId,
                                $"Ваш средний доход = {averageIncome}, прожиточный минимум по вашему региону = {livingWage}.",
                                cancellationToken);
                        }
                    }
                    catch (Exception exp)
                    {
                        await SendTextMessage(botClient, message.ChatId,
                            $"Некорректно указан доход (пример: 34688)",
                            cancellationToken);
                    }
                }
            }
        }

        public async Task ResponsCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = new UpdateModel(update);

            var lastMessage = new DatabaseController().GetLastMessage(message.ChatId);

            if (lastMessage != null)
            {
                if (lastMessage.Type == "RegionName")
                {
                    using (var db = new DatabaseContext())
                    {
                        var region = db.Regions.FirstOrDefault(p => p.Name == message.ButtonData!);

                        if (region != null)
                        {
                            //Добавляем регион для ChatId в БД
                            new DatabaseController().InsertAnswer(message.ChatId, regionId: region.Id);

                            await SendTextMessage(botClient, message.ChatId,
                                "Введите количество проживающих по факту человек (только тех, которые официально прописаны и проживают)",
                                cancellationToken, "NumberPeople");
                        }
                        else
                        {
                            await SendTextMessage(botClient, message.ChatId,
                                "Не удалось найти регион в базе данных.",
                                cancellationToken);
                        }
                    }
                }

                if (lastMessage.Type == "CategoryPeople")
                {
                    var dbController = new DatabaseController();

                    //добавляем человека, если еще не все записаны
                    dbController.InsertCategoryPeople(message.ChatId, message.ButtonData!);

                    //получаем количество человек + количество добавленных
                    var numberPeople = dbController.GetNumberPeople(message.ChatId);
                    var addedPeople = dbController.GetAddedPeople(message.ChatId);

                    if (addedPeople < numberPeople)
                    {
                        var toolbar = new TelegramToolbar();

                        toolbar.AddInlineKeyboardButton($"Трудоспособный человек", "Worker");
                        toolbar.AddInlineKeyboardButton($"Ребенок младше 18 лет", "Child");
                        toolbar.AddInlineKeyboardButton($"Пенсионер", "Pensioner");

                        var markup = toolbar.keyboardMarkup;

                        await EditMessageText(botClient, message.ChatId, message.MessageId,
                            $"Укажите к какой категории относится человек №{addedPeople + 1}",
                            markup!, cancellationToken);
                    }
                    else
                    {
                        await SendTextMessage(botClient, message.ChatId,
                            $"Далее, необходимо указать суммарный доход всех проживающих по месяцам.",
                            cancellationToken, "MonthlyIncome");
                        await SendTextMessage(botClient, message.ChatId,
                            $"Укажите доход за {GetMonth(1)} (пример: 34688)",
                            cancellationToken);
                    }
                }
            }
        }

        public String GetMonth(Int32 count)
        {
            var dt = DateTime.Now.AddMonths(-(4 - count));

            var month = dt.Month;
            var year = dt.Year;

            var res = "";

            switch (month)
            {
                case 1: res = $"Январь {year} года"; break;
                case 2: res = $"Февраль {year} года"; break;
                case 3: res = $"Март {year} года"; break;
                case 4: res = $"Апрель {year} года"; break;
                case 5: res = $"Май {year} года"; break;
                case 6: res = $"Июнь {year} года"; break;
                case 7: res = $"Июль {year} года"; break;
                case 8: res = $"Август {year} года"; break;
                case 9: res = $"Сентябрь {year} года"; break;
                case 10: res = $"Октябрь {year} года"; break;
                case 11: res = $"Ноябрь {year} года"; break;
                case 12: res = $"Декабрь {year} года"; break;
                default: break;
            }

            return res;
        }

        public List<String> CheckRegion(string answerText)
        {
            using (var db = new DatabaseContext())
            {
                var regions = db.Regions.ToList();

                var enableRegions = new List<String>();

                var answerLength = answerText.Length;
                var coincidence = 0;
                var startCheck = 0;

                foreach (var region in regions)
                {
                    for (int i = 0; i < answerText.ToLower().ToArray().Length; i++)
                    {
                        for (int j = startCheck; j < region.Name.ToLower().ToArray().Length; j++)
                        {
                            var symbolMsg = answerText.ToLower().ToArray()[i];
                            var symbolDb = region.Name.ToLower().ToArray()[j];
                            if (answerText.ToLower().ToArray()[i] == region.Name.ToLower().ToArray()[j])
                            {
                                coincidence++;
                                startCheck = j + 1;
                                break;
                            }
                        }
                    }
                    if (coincidence > answerLength - 2)
                    {
                        enableRegions.Add(region.Name);
                    }
                    coincidence = 0;
                    startCheck = 0;
                }

                return enableRegions;
            }
        }

        public Int32 GetAverageIncome(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                var income = db.MonthlyIncome.Where(p => p.ChatId == chatId);
                var sum = 0;
                foreach (var item in income)
                {
                    sum += item.Amount;
                }
                return sum / 3;
            }
        }
        public Int32 GetLivingWage(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                var answer = db.Answers.FirstOrDefault(p => p.ChatId == chatId);
                var region = db.Regions.FirstOrDefault(p => p.Id == answer!.RegionId);
                var costLiving = db.CostLiving.FirstOrDefault(p => p.RegionId == region!.Id);
                
                var people = db.CategoryPeople.Where(p => p.ChatId == chatId);

                var sum = 0;
                foreach (var item in people)
                {
                    if (item.Category == "Worker")
                    {
                        sum += costLiving!.Worker;
                    }
                    if (item.Category == "Child")
                    {
                        sum += costLiving!.Child;
                    }
                    if (item.Category == "Pensioner")
                    {
                        sum += costLiving!.Pensioner;
                    }
                }
                return sum;
            }
        }
    }
}
