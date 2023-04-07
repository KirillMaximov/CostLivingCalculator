using Telegram.Bot;
using CostLivingCalculator.Create;

#region Create bots

var telegramToken = "6048365580:AAHQ3OmtUiJ2AKQrDbDxCPUUFsvi5ueaBLw";
CalculatorBot bot = new CalculatorBot(telegramToken); //CostLivingCalculatorBot
var botName = await bot.BotClient.GetMeAsync();
Console.WriteLine($"Начинаем работу с @{botName.Username}");

#endregion

#region Задержка для работы консоли

await Task.Delay(int.MaxValue);

#endregion

#region Удаление токена после работы

bot.Cancellation.Cancel();

#endregion