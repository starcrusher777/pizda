using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Telegram.Bot.Types.Enums.UpdateType;
using CallbackQuery = Telegram.Bot.Types.CallbackQuery;

namespace HSRBot
{
    class Program
    {
        
        static ITelegramBotClient bot = new TelegramBotClient("6528515375:AAGH2grbOdX0Iee12YfePk0Ihbh51W_O95I");

        static List<(long, string)> stages = new List<(long, string)>();
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long? numbersChatId = null;
            long? daysChatId = null;
            var message = update.Message;
            
            var chatStage = await getChatStage(message.Chat.Id);
            if (!string.IsNullOrEmpty(chatStage))
            {
                switch (chatStage)
                {
                    case "Посчитать прыжки": await HandleReceivedNumbersAsync(message);
                        break;
                    case "Посчитать колличество нефрита через X дней": await HandleReceivedDaysAsync(message);
                        break;
                    case "В главное меню": await toMainMenu(message);
                        break;
                }
                return;
            }

            if (message.Text == "Посчитать прыжки")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы выбрали посчитать прыжки, введите колличество нефрита");
                await setChatInStage(message.Chat.Id, "Посчитать прыжки");
                return;
            }

            if (message.Text == "Посчитать колличество нефрита через X дней")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы выбрали посчитать колличество нефрита через X дней, введите число дней");
                await setChatInStage(message.Chat.Id, "Посчитать колличество нефрита через X дней");
                return;
            }
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
           
            if (update.Type == UpdateType.Message)
            {
                
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Выбери клавиатуру:\n" +
                                                                       "/reply\n");
                    return;
                }

                if (message.Text == "/reply")
                {
                    var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Посчитать прыжки"),
                                new KeyboardButton("Посчитать колличество нефрита через X дней"),
                            },
                            new KeyboardButton[]
                            {
                                new KeyboardButton("В главное меню")
                            },
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Test 4")
                            }
                        })
                    {
                        ResizeKeyboard = true,
                        };
                    await botClient.SendTextMessageAsync(message.Chat, "Что вас интересует?",
                        replyMarkup: replyKeyboard);    
                        
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
            }
        }
        static async Task HandleReceivedDaysAsync(Message message1)
        {
            var daysChatId = message1.Chat.Id;

            if (message1.Text != null)
            {
                if (double.TryParse(message1.Text, out double inputNumber2))
                {
                    var f2p = inputNumber2 * 60;
                    var not = inputNumber2 * 150;
                    await bot.SendTextMessageAsync(daysChatId, $"Колличество нефрита через {inputNumber2} дней: С дейликов - {f2p}, с пропуском - {not}");
                    await removeChatFromStage(daysChatId);
                }
                else
                {
                    await bot.SendTextMessageAsync(daysChatId, "Пожалуйста, введите число в правильном формате.");
                }
            }
        }
        static async Task HandleReceivedNumbersAsync(Message message)
        {
            var NumbersChatId = message.Chat.Id;

            if (message.Text != null)
            {
                if (double.TryParse(message.Text, out double inputNumber))
                {
                    var result1 = (int)Math.Floor(inputNumber / 160);
                    double result2 = inputNumber - (result1 * 160);
                    await bot.SendTextMessageAsync(NumbersChatId, $"Колличество прыжков: {result1} + {result2} нефрита остаток");
                    await removeChatFromStage(NumbersChatId);
                }
                else
                {
                    await bot.SendTextMessageAsync(NumbersChatId, "Пожалуйста, введите число в правильном формате.");
                }
            }
        }
        static async Task setChatInStage(long chatId, string stageName)
        {
            stages.Add(new(chatId, stageName));
        }
        static async Task<string> getChatStage(long chatId)
        {
            return stages.FirstOrDefault(x => x.Item1 == chatId).Item2;
        }
        static async Task removeChatFromStage(long chatId)
        {
            stages.RemoveAll(x => x.Item1 == chatId);
        }

        static async Task toMainMenu(Message message)
        {
            if (message.Text == "В главное меню")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(
                    new List<KeyboardButton[]>()
                    {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Посчитать прыжки"),
                            new KeyboardButton("Посчитать колличество нефрита через X дней"),
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("В главное меню")
                        }
                    })
                {
                    ResizeKeyboard = true,
                };
                await bot.SendTextMessageAsync(message.Chat, "Что вас интересует?",
                    replyMarkup: replyKeyboard);    
                return;
            }
        }

        public static void Main(string[] args) 
            {
            Console.WriteLine("Let`s go " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = {  }, 
                };

            bot.StartReceiving(
                (client, update, arg3) => HandleUpdateAsync(client, update, arg3),
                (client, exception, arg3) => HandleErrorAsync(client, exception, arg3),
                receiverOptions,
                cancellationToken
                              );

            Console.ReadLine();
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}