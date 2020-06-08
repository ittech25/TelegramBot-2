using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Net;
using Telegram.Bot.Args;
using System.Threading;
using System.Data.SqlClient;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using System.Diagnostics;

namespace TelegramBot
{
    class Program
    {
        static readonly string token = "1157812716:AAHj53XRVRj0f_F1pJrCpouS-EzFuCINJHE";

        static readonly string ip = "86.123.88.138";
        static readonly int port = 3128;

        static ITelegramBotClient botClient;

        static DataBase dataBase;
        static UsersDataBase usersDataBase;
        static void Main(string[] args)
        {
            dataBase = new DataBase();

            usersDataBase = new UsersDataBase();


            var proxy = new WebProxy(ip, port);
            proxy.Credentials = new NetworkCredential();

            botClient = new TelegramBotClient(token, proxy);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}");

            botClient.OnMessage += Bot_OnMessage; // Когда приходит сообщение

            botClient.OnUpdate += Bot_OnUpdate; //Принимаемые сообщения

            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);


            Console.ReadKey();
        }

        static async void Bot_OnUpdate(object sender, UpdateEventArgs e) //Принимаемые сообщения
        {
            //Добавить обработку типа кнопки (выбор ситуации либо выбор самой кнопки)
            if (e.Update.CallbackQuery != null)
            {

                var data = e.Update.CallbackQuery.Data;
                var chat_id = e.Update.CallbackQuery.From.Id;

                if (data != "/start" && data != "/about")
                {
                    var CODE = data.Split('.')[0];
                    var VALUE = data.Split('.')[1];
                    var SITUATION = data.Split('.')[2];

                    switch (CODE) // Code.*int*.Situation
                    {
                        case "Situation":

                            LoadSituationById(VALUE, chat_id);

                            break;
                        case "Choice":

                            LoadChoices(SITUATION, chat_id, VALUE);

                            break;
                    }
                }
                else
                {
                    if(data == "/start")
                    {
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(GetListButtons(chat_id));

                        string text = "Доступные ситуации";

                        await botClient.SendTextMessageAsync(
                              chatId: chat_id,
                              text: text,
                              replyMarkup: inlineKeyboardMarkup
                              );
                        return;
                    }
                }

                //if (data.Length == 1)
                //{
                //    LoadSituationById(data, chat_id);
                //}
                //else
                //{
                //    LoadChoices()
                //}
            }
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(GetListButtons(e.Message.Chat.Id));

            string text = "Доступные ситуации";
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                switch(e.Message.Text)
                {
                    case "/start":
                        {
                            if(!usersDataBase.user_exists_in_db(e.Message.Chat.Id)) // Пользователя нет в базе данных.
                            {
                                usersDataBase.add_new_user(e.Message.Chat.Id);

                                text = "Впервые тут? Ничего страшного, я тебя уже добавил в списки!";

                                await botClient.SendTextMessageAsync(
                                  chatId: e.Message.Chat,
                                  text: text
                                );

                                text = "Доступные ситуации";
                            }

                            await botClient.SendTextMessageAsync(
                              chatId: e.Message.Chat.Id,
                              text: text,
                              replyMarkup: inlineKeyboardMarkup
                              );

                        } 
                        break;
                    case "/about":
                        {
                            text = "Я создан в УрФУ, привет мир!";

                            await botClient.SendTextMessageAsync(
                              chatId: e.Message.Chat,
                              text: text
                            );
                        } break;
                    //case "/test":
                    //    {
                    //        usersDataBase.situation_done(e.Message.Chat.Id, 4);
                    //    }
                    //    break;
                }
            }
        }

        static List<List<InlineKeyboardButton>> GetListButtons(long chat_id)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>(); // Кнопки для выбора ситуации.
            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();

            var counter = 1;

            for(var i = 0; i < 10; i++)
            {
                keyboard.Add(new List<InlineKeyboardButton>());
                for(var j = 0; j < 3; j++)
                {
                    if (usersDataBase.is_situation_done(chat_id, counter))
                        keyboard[i].Add(new InlineKeyboardButton() { Text = "Пройдено ", CallbackData = "Situation." + counter.ToString() + "." + counter.ToString() });
                    else
                        keyboard[i].Add(new InlineKeyboardButton() { Text = "Тест № " + counter, CallbackData = "Situation." + counter.ToString() + "." + counter.ToString()});
                    counter++;
                }
            }

            return keyboard;
        }
        static void LoadSituationById(string data, long chat_id)
        {
            LoadChoices(data, chat_id, "1");
        }

        static async void LoadChoices(string data, long chat_id, string start_choice) // Ситуация с выборами.
        {
            var description = dataBase.recive_text(data, start_choice) + "\n";

            var childs = dataBase.get_childs(data, start_choice);

            if(childs[0] == "0" || childs[0] == "-1")
            {
                if (childs[0] == "-1")
                {
                    usersDataBase.situation_done(chat_id, int.Parse(data));
                    var backButton = new InlineKeyboardButton() { Text = "Успешно, назад", CallbackData = "/start" };
                    var local_markup = new InlineKeyboardMarkup(backButton);

                    await botClient.SendTextMessageAsync(
                                  chatId: chat_id,
                                  text: description,
                                  replyMarkup: local_markup
                    );
                }
                else
                {
                    var backButton = new InlineKeyboardButton() { Text = "Назад", CallbackData = "/start" };
                    var local_markup = new InlineKeyboardMarkup(backButton);

                    await botClient.SendTextMessageAsync(
                                  chatId: chat_id,
                                  text: description,
                                  replyMarkup: local_markup
                    );
                }

                return;
            }

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();


            for(var i = 0; i < childs.Length; i++)
            {
                description += $"<b>\n{i}. " + dataBase.recive_text(data, childs[i]) + "</b>";
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = i.ToString(),
                    CallbackData = "Choice." + dataBase.get_childs(data, childs[i])[0] + "." + data // Всегда отсылают к одному варианту.
                });
            }

            var markup = new InlineKeyboardMarkup(buttons);

            await botClient.SendTextMessageAsync(
                              chatId: chat_id,
                              text: description,
                              replyMarkup: markup,
                              parseMode: ParseMode.Html
            );

        }

    }
}
