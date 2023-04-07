using CostLivingCalculator.DataAccess;
using CostLivingCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CostLivingCalculator.DataBase
{
    internal class DatabaseController
    {
        public void InserMessage(Int64 chatId, Int32 messageId, String? type = null)
        {
            using (var db = new DatabaseContext())
            {
                var message = new MessageModel()
                {
                    ChatId = chatId,
                    MessageId = messageId,
                    Type = type,
                    Created = DateTime.Now
                };

                db.Messages.Add(message);
                db.SaveChanges();
            }
        }

        public MessageModel GetLastMessage(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                return db.Messages.OrderBy(m => m.Created).LastOrDefault(p => p.ChatId == chatId && !String.IsNullOrEmpty(p.Type))!;
            }
        }

        public void InsertAnswer(Int64 chatId, Int32? regionId = null, Int32? numberPeople = null)
        {
            using (var db = new DatabaseContext())
            {
                var answer = db.Answers.FirstOrDefault(p => p.ChatId == chatId);

                if (answer != null)
                {
                    if (regionId != null) { answer.RegionId = regionId ?? 0; }
                    if (numberPeople != null) { answer.NumberPeople = numberPeople ?? 0; }
                }
                else
                {
                    var newAnswer = new AnswerModel();

                    newAnswer.ChatId = chatId;
                    if (regionId != null) { newAnswer.RegionId = regionId ?? 0; }
                    if (numberPeople != null) { newAnswer.NumberPeople = numberPeople ?? 0; }
                    newAnswer.CreateDate = DateTime.Now;

                    db.Answers.Add(newAnswer);
                }

                db.SaveChanges();
            }
        }

        public Int32 GetNumberPeople(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                var numberPeople = db.Answers.FirstOrDefault(p => p.ChatId == chatId);

                return numberPeople != null ? numberPeople.NumberPeople : 0;
            }
        }

        public Int32 GetAddedPeople(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                var people = db.CategoryPeople.Where(p => p.ChatId == chatId);

                return people.Count();
            }   
        }

        public void InsertCategoryPeople(Int64 chatId, String category)
        {
            using (var db = new DatabaseContext())
            {
                var newPerson = new CategoryPeopleModel();

                newPerson.ChatId = chatId;
                newPerson.Category = category;

                db.CategoryPeople.Add(newPerson);
                db.SaveChanges();
            }
        }

        public Int32 GetAddedIncome(Int64 chatId)
        {
            using (var db = new DatabaseContext())
            {
                var income = db.MonthlyIncome.Where(p => p.ChatId == chatId);

                return income.Count();
            }
        }

        public void InsertMonthlyIncome(Int64 chatId, Int32 amount)
        {
            using (var db = new DatabaseContext())
            {
                var newIncome = new MonthlyIncomeModel();

                newIncome.ChatId = chatId;
                newIncome.Amount = amount;

                db.MonthlyIncome.Add(newIncome);
                db.SaveChanges();
            }
        }

        internal void ClearDataBase(long chatId)
        {
            using (var db = new DatabaseContext())
            {
                var people = db.CategoryPeople.Where(p => p.ChatId == chatId);
                var incoming = db.MonthlyIncome.Where(p => p.ChatId == chatId);
                var answers = db.Answers.Where(p => p.ChatId == chatId);
                var messages = db.Messages.Where(p => p.ChatId == chatId);

                if (people.Any()) db.CategoryPeople.RemoveRange(people);
                if (incoming.Any()) db.MonthlyIncome.RemoveRange(incoming);
                if (answers.Any()) db.Answers.RemoveRange(answers);
                if (messages.Any()) db.Messages.RemoveRange(messages);

                db.SaveChanges();
            }
        }
    }
}
