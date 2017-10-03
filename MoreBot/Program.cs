using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoreBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("474545390:AAHU8XYrFNbsFPMpIklVtqk9NSiCmG3-Fjk");
        private static readonly Random random = new Random();
        private static readonly DateTime runTime = DateTime.Now;
        private static readonly StreamWriter file = new StreamWriter("d:\\history.txt");
        private static int answerPosibility = 5;
        private static int huiPosibility = 10;
        private static int imagePosibility = 25;

        private static void Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Bot.OnMessage += BotOnMessage;
            Bot.StartReceiving();
            while (true)
            {
                var text = Console.ReadLine();
                if (text == "exit")
                    break;
                Bot.SendTextMessageAsync(Constants.MoreChatId, text);
            }
            Bot.StopReceiving();
        }

        private static void BotOnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.Message.Date < runTime)
                    return;
                if (e.Message.Type == MessageType.PhotoMessage)
                {
                    OnPhoto(e);
                }

                if (e.Message.Type == MessageType.TextMessage)
                {
                    OnText(e);
                }
            }
            catch (Exception ex)
            {
                file.WriteLine(ex);
            }
        }

        private static void OnPhoto(MessageEventArgs e)
        {
            if (Can(imagePosibility))
            {
                var answers = Constants.Phrases
                    .Where(x => x.UserId == User.Image)
                    .ToList();
                Bot.SendTextMessageAsync(e.Message.Chat.Id, answers[random.Next(answers.Count)].Text);
            }
        }

        private static void OnText(MessageEventArgs e)
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
            else if (e.Message.Text.ToLower().Contains("частота"))
            {
                try
                {
                    var words = e.Message.Text.ToLower().Split(' ');
                    switch (words[1])
                    {
                        case "відповідь":
                            answerPosibility = int.Parse(words[2]);
                            break;
                        case "хуй":
                            huiPosibility = int.Parse(words[2]);
                            break;
                        case "картинка":
                            imagePosibility = int.Parse(words[2]);
                            break;
                        default: return;
                    }
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Частота оновлена.");
                }
                catch
                {
                }

            }
            else if (e.Message.Text.ToLower().EndsWith("триста") || e.Message.Text.ToLower().EndsWith("300"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "отсоси у тракториста");
            }
            else if (e.Message.Text.ToLower().Contains("ахах"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "ахахахах");
            }
            else if (e.Message.Text.ToLower().EndsWith("ізі"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Риал толк");
            }
            else if (e.Message.Text.ToLower().EndsWith(" нет"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "пидора ответ");
            }
            else if (e.Message.Text.ToLower().Contains("бухати"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id,
                    e.Message.Date.DayOfWeek != DayOfWeek.Friday
                        ? "Хтось сказав бухати? Я завжди не проти"
                        : "Бухати?? Пфф.. звііісно! сьогоодні ж пятниця, котіки, чекаю вас всіх в СпортПабі в 19.00 ");
            }
            else if (e.Message.Text.ToLower().Contains("макс"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Аве Макс!!!");
            }
            else if (Can(huiPosibility))
            {
                if (e.Message.Text.Length >= 4)
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "хуй" + e.Message.Text.Substring(e.Message.Text.Length - 4));
            }
            else if (Can(answerPosibility))
            {
                var answers = Constants.Phrases
                    .Where(x => x.UserId == User.Any || (int)x.UserId == e.Message.From.Id)
                    .ToList();
                Bot.SendTextMessageAsync(e.Message.Chat.Id, answers[random.Next(answers.Count)].Text);
            }
        }

        static async Task<string> GetDamn(string name)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://damn.ru/?name=" + name + "&sex=m"))
                {
                    using (var content = response.Content)
                    {
                        var myContent = await content.ReadAsStringAsync();
                        return getDamnFromResponse(myContent);
                    }
                }
            }
        }

        private static bool Can(int percent)
        {
            return random.Next(100) < percent;
        }

        private static String getDamnFromResponse(String s)
        {
            if (!s.Contains("<div class=\"damn\""))
                return null;
            var start = s.IndexOf("<div class=\"damn\"");
            start = s.IndexOf(">", start + 1);
            start++;
            var end = s.IndexOf("</div", start);
            var withSpan = s.Substring(start, end - start);
            withSpan = withSpan.Replace("<span class=\"name\">", "");
            withSpan = withSpan.Replace("&mdash; ", "");
            return withSpan.Replace("</span>", "");
        }
    }
}

