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

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long? numbersChatId = null;
            long? daysChatId = null;
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
           
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
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
                                new KeyboardButton("Посчитать колличество нефрите через X дней"),
                            },
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Test 3")
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
                        return;
                }
                
                
                try
                            {
                                
                                switch (update.Type)
                                {
                                    case UpdateType.Message:
                                    {
                                        switch (message.Type)
                                        {
                                            case MessageType.Text:
                                            {
                                                if (message.Text == "Посчитать прыжки")
                                                {
                                                    numbersChatId = message.Chat.Id;
                                                    await AskForNumbersAsync(numbersChatId.Value);
                                                }
                                                else
                                                {
                                                    await HandleReceivedNumbersAsync(message);
                                                }
                                                
                                                 if (message.Text == "Посчитать колличество нефрите через X дней")
                                                {
                                                    daysChatId = message.Chat.Id;
                                                    await AskForDaysAsync(daysChatId.Value);
                                                }
                                                else
                                                {
                                                    await HandleReceivedDaysAsync(message);
                                                }
                                                
                                                if (message.Text == "Test 2")
                                                {
                                                    //await botClient.SendTextMessageAsync(message.Chat, "Что вас интересует?", replyMarkup: inlineKeyboard);
                                                    await botClient.SendTextMessageAsync(message.Chat, "Var 2", replyToMessageId: message.MessageId);
                                                    return;
                                                }
                                                return;
                                            }
                                            }        
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                
            }
            static async Task AskForNumbersAsync(long numberschatId)
            {
                var message = "Пожалуйста, введите число нефрита:";
                var replyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Посчитать"),
                    },
                });

                await bot.SendTextMessageAsync(numberschatId, message, replyMarkup: replyMarkup);
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
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(NumbersChatId, "Пожалуйста, введите число в правильном формате.");
                    }
                }
            }
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;

                if (message.Text == "Посчитать колличество прыжков")
                {
                    await AskForNumbersAsync((numbersChatId.Value));
                }
                else
                {
                    await HandleReceivedNumbersAsync(message);
                }
            }
            
            static async Task AskForDaysAsync(long dayschatId)
            {
                var message = "Пожалуйста, введите число дней:";
                var replyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Посчитать число дней"),
                    },
                });

                await bot.SendTextMessageAsync(dayschatId, message, replyMarkup: replyMarkup);
            }
            
            static async Task HandleReceivedDaysAsync(Message message1)
            {
                var daysChatId = message1.Chat.Id;

                if (message1.Text != null)
                {
                    if (double.TryParse(message1.Text, out double inputNumber2))
                    {
                        var result3 = inputNumber2 * 60;
                        await bot.SendTextMessageAsync(daysChatId, $"Колличество нефрита через {inputNumber2} дней: {result3}");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(daysChatId, "Пожалуйста, введите число в правильном формате.");
                    }
                }
            }
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;

                if (message.Text == "Посчитать")
                {
                    await AskForDaysAsync(daysChatId.Value);
                }
                else
                {
                    await HandleReceivedDaysAsync(message);
                }
            }
            
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
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