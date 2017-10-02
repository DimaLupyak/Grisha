using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoreBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("474545390:AAG5TM9OrDUSV6jY1fdj0kHjv7rfNkZsHNk");
        private static readonly Random random = new Random();
        private static readonly DateTime runTime = DateTime.Now;
        private static readonly StreamWriter file = new StreamWriter("d:\\history.txt");

        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessage;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        async static Task<string> GetDamn(string name)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://damn.ru/?name=" + name + "&sex=m"))
                {
                    using (var content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                        Console.WriteLine(getDamnFromResponse(myContent));
                        return getDamnFromResponse(myContent);
                    }
                }
            }
        }

        private static String getDamnFromResponse(String s)
        {
            if (!s.Contains("<div class=\"damn\" data-id=\""))
                return null;
            var start = s.IndexOf("<div class=\"damn\" data-id=\"");
            var end = s.IndexOf("</div", start + 1);
            var withSpan = s.Substring(start + 36, end - start - 36);
            withSpan = withSpan.Replace("<span class=\"name\">", "");
            withSpan = withSpan.Replace("&mdash; ", "");
            return withSpan.Replace("</span>", "");
        }

        private static void BotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
       {
            try
            {

                if (e.Message.Date < runTime)
                    return;
                if (e.Message.Type == MessageType.PhotoMessage)
                {
                    switch (random.Next(30))
                    {
                        case 0:
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "найс картінка");
                            break;
                        case 2:
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Щось це не похоже на годний мемас");
                            break;
                        case 3:
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "макс задизайнив би краще");
                            break;
                        case 4:
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "вау, наче богом намальовано");
                            break;
                        case 5:
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "зря ти це кинув");
                            break;
                    }
                }

                if (e.Message.Type == MessageType.TextMessage)
                {
                    Console.WriteLine(e.Message.Date + "  |  " + e.Message.Type + " from " + e.Message.From.FirstName + "  " + e.Message.From.LastName);
                    file.WriteLine(e.Message.Text + "  |  " + e.Message.Date + " | " + e.Message.From.FirstName + "  " + e.Message.From.LastName + " = " + e.Message.From.Id);
                    if (e.Message.Text.ToLower().Contains("обізви"))
                    {
                        var words = e.Message.Text.ToLower().Split(' ');

                        var name = words[words.ToList().IndexOf("обізви") + 1];
                        if (!string.IsNullOrEmpty(name))
                        {
                            var damn = GetDamn(name).Result;
                            if (!string.IsNullOrEmpty(damn))
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, damn);
                            }
                        }
                    }


                    else if (e.Message.Text.ToLower().Contains("бухати"))
                    {
                        if (e.Message.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Бухати?? Пфф.. звііісно! сьогоодні ж пятниця, котіки, чекаю вас всіх в СпортПабі в 19.00 ");
                        }
                        else
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Хтось сказав бузхати? Я завжди не проти");
                        }
                    }
                    else if (e.Message.Text.ToLower().Contains("макс"))
                    {
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Аве Макс!!!");
                    }
                    else if (random.Next(15) == 0)
                    {
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "хуй" + e.Message.Text.Substring(e.Message.Text.Length - 3));
                    }
                    else if (e.Message.From.Id == (int) User.Dudko)
                    {
                        switch (random.Next(30))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Псс, Валєрці привіт!");
                                break;
                            case 2:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Скажеш це свому ананасу");
                                break;
                            case 3:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Славік, діставай!");
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Mudra)
                    {
                        switch (random.Next(40))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Ти що, сама мудра тут?");
                                break;
                            case 1:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Соонь, ти знову тут флудиш?");
                                break;
                            case 4:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Ух яка красотка тут <3");
                                break;
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Max)
                    {
                        switch (random.Next(30))
                        {
                            case 0:
                            case 1:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Мааакс, спасіба, що живий!");
                                break;
                            case 2:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Ооо, цей парєнь явно любить випити!");
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Vadim)
                    {
                        switch (random.Next(20))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Чувак, класна попа!");
                                break;
                            case 1:
                            case 2:
                            case 3:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Забей!");
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Gedz)
                    {
                        switch (random.Next(20))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Їбало треба мати попроще!");
                                break;
                            case 1:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Огооо яка машина!!!");
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Yarik)
                    {
                        switch (random.Next(15))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Мені здається, чи фронтендом засмерділо?");
                                break;
                        }
                    }
                    else if (e.Message.From.Id == (int)User.Lupiak)
                    {
                        switch (random.Next(20))
                        {
                            case 0:
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Мамуля написала <3!");
                                break;
                        }
                    }
                }
            }
            catch { }
        }
    }
}

