using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoreBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("474545390:AAHU8XYrFNbsFPMpIklVtqk9NSiCmG3-Fjk");
        private static readonly Random random = new Random();
        private static readonly DateTime runTime = DateTime.Now;
        private static readonly StreamWriter file = new StreamWriter("d:\\history.txt");
        private static int answerPosibility = 1;
        private static int huiPosibility = 1;
        private static int imagePosibility = 2;
        private static int stickerPosibility = 1;
        private static bool alive = true;
        private static readonly List<Voice> VoiceList = new List<Voice>();
        private static void Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Bot.OnMessage += BotOnMessage;
            Bot.OnCallbackQuery += OnBotCallbackQuery;
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
            if (e.Message.Text.ToLower().Contains("гріша привіт"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "привііііт!");
                alive = true;
            }
            else if (!alive)
            {
                return;
            }
            else if (e.Message.Text.ToLower().Contains("гріша пока"))
            {
                var damn = GetDamn(e.Message.From.FirstName).Result;
                if (!string.IsNullOrEmpty(damn))
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, damn);
                }
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "пока =(");
                alive = false;
            }

            else if (e.Message.Text.ToLower().Contains("обізви"))
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
                        case "стікер":
                            stickerPosibility = int.Parse(words[2]);
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
            else if (e.Message.Text.ToLower().EndsWith(" ="))
            {
                Bot.SendTextMessageAsync(
                    e.Message.Chat.Id, 
                    GetCalc(e.Message.Text.ToLower().Substring(0, e.Message.Text.Length-2)).Result
                    );
            }
            else if (e.Message.Text.ToLower().EndsWith("совпадение?") || e.Message.Text.ToLower().EndsWith("співпадіння?"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "не думаю!");
            }
            else if (e.Message.Text.ToLower().Contains("ахах"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "ахахахах");
            }
            else if (e.Message.Text.ToLower().Contains("анекдот"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Лизав мужик пизду.");
            }
            else if (e.Message.Text.ToLower().Contains("випадковий відосік"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, GetRandomVideo().Result);
            }
            else if (e.Message.Text.ToLower().Contains("коте"))
            {
                if (e.Message.Text.ToLower().Split(" ,.&!?-0123456789*-+//_^:;\"\'".ToCharArray()).Any(x => x == "коте"))
                {
                    var type = random.Next(5) == 0 ? "jpg" : "gif";
                    using (var stream = new MemoryStream(GetCat(type).Result))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        Task x;
                        if (type == "gif")
                        {
                            x = Bot.SendDocumentAsync(e.Message.Chat.Id, new FileToSend("cat." + type, stream));
                        }
                        else
                        {
                            x = Bot.SendPhotoAsync(e.Message.Chat.Id, new FileToSend("cat." + type, stream));
                        }

                        while (x.Status == TaskStatus.WaitingForActivation)
                        {
                            Thread.Sleep(1000);
                        }
                    }   
                }
                else
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Хуй тобі а не кота, розумний самий? хоч кота, пиши коте окремо і не вийобуйся.");
                }           
            }
            else if (e.Message.Text.ToLower().Contains("хуй"))
            {
                var stickerSet = Bot.GetStickerSetAsync("Thngs");
                Bot.SendStickerAsync(e.Message.Chat.Id, new FileToSend(stickerSet.Result.Stickers[random.Next(stickerSet.Result.Stickers.Count)].FileId));
            }
            else if (e.Message.Text.ToLower().Contains("лисий"))
            {
                var stickerSet = Bot.GetStickerSetAsync("johnnysinsbrazzers");
                Bot.SendStickerAsync(e.Message.Chat.Id, new FileToSend(stickerSet.Result.Stickers[random.Next(stickerSet.Result.Stickers.Count)].FileId));
            }
            else if (e.Message.Text.ToLower().Contains("секс"))
            {
                var stickerSet = Bot.GetStickerSetAsync("SigmundFreud");
                Bot.SendStickerAsync(e.Message.Chat.Id, new FileToSend(stickerSet.Result.Stickers[random.Next(stickerSet.Result.Stickers.Count)].FileId));
            }
            else if (e.Message.Text.ToLower().EndsWith("омг"))
            {
                var stickerSet = Bot.GetStickerSetAsync("More_Faces");
                Bot.SendStickerAsync(e.Message.Chat.Id, new FileToSend(stickerSet.Result.Stickers[1].FileId));
            }
            else if (e.Message.Text.ToLower().EndsWith("ізі"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Риал толк");
            }
            else if (e.Message.Text.ToLower().EndsWith("нет") || e.Message.Text.ToLower().EndsWith("нєт"))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "пидора отвєт");
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
            else if (Can(answerPosibility))
            {
                var answers = Constants.Phrases
                    .Where(x => x.UserId == User.Any || (int)x.UserId == e.Message.From.Id)
                    .ToList();
                Bot.SendTextMessageAsync(e.Message.Chat.Id, answers[random.Next(answers.Count)].Text);
            }
            if (e.Message.Text.ToLower().Contains("голосованіє") || e.Message.Text.ToLower().Contains("голосування"))
            {
                try
                {
                    var answer = e.Message.Text.Split(':')[1];
                    var variants = e.Message.Text.Split(':')[2].Split(',');

                    var buttons = new InlineKeyboardCallbackButton[variants.Length];
                    for (var i = 0; i < variants.Length; i++)
                    {
                        buttons[i] = new InlineKeyboardCallbackButton(variants[i], "callbackVoice" + i);
                    }

                    var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[][] { buttons });

                    var isOpen = e.Message.Text.ToLower().Contains("відкрите");
                    var mess = Bot.SendTextMessageAsync(e.Message.Chat.Id, answer, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, keyboard).Result;
                    VoiceList.Add(new Voice(mess.MessageId, answer, variants, isOpen));
                }
                catch (Exception ex)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Це так не работає");
                }
            }
            else if (Can(stickerPosibility))
            {
                var stickerSet1 = Bot.GetStickerSetAsync("More_Faces");
                var stickerSet2 = Bot.GetStickerSetAsync("Vit2005");
                var stickerSet3 = Bot.GetStickerSetAsync("BlueRobots");
                var stickerSet4 = Bot.GetStickerSetAsync("Druzhko");
                var stickerSet5 = Bot.GetStickerSetAsync("terebonk_2");
                var stickers = stickerSet1.Result.Stickers;
                stickers.AddRange(stickerSet2.Result.Stickers);
                stickers.AddRange(stickerSet3.Result.Stickers);
                stickers.AddRange(stickerSet4.Result.Stickers);
                stickers.AddRange(stickerSet5.Result.Stickers);
                Bot.SendStickerAsync(e.Message.Chat.Id, new FileToSend(stickers[random.Next(stickers.Count)].FileId));
            }
            else if (Can(huiPosibility))
            {
                if (e.Message.Text.Length >= 4)
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "хуй" + e.Message.Text.Substring(e.Message.Text.Length - 4));
            }
        }

        private static async void OnBotCallbackQuery(object ob, CallbackQueryEventArgs ev)
        {
            var message = ev.CallbackQuery.Message;


            if (ev.CallbackQuery.Data.Contains("callbackVoice"))
            {
                try
                {
                    int num = int.Parse(ev.CallbackQuery.Data[ev.CallbackQuery.Data.Length - 1].ToString());
                    {
                        var currentVoice = VoiceList.First(gol => gol.MessageId == ev.CallbackQuery.Message.MessageId);
                        var variants = currentVoice.Answers;
                        var buttons = new InlineKeyboardCallbackButton[variants.Length];

                        if (currentVoice.Votes.ContainsKey(ev.CallbackQuery.From.FirstName + ev.CallbackQuery.From.LastName))
                        {
                            if (currentVoice.IsOpen)
                            {
                                var all = string.Format("{0} ({1})", currentVoice.Answers[num],
                                    string.Join(", ", 
                                        currentVoice.Votes
                                            .Where(item => item.Value == num)
                                            .Select(item => item.Key)));
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, all);
                            }
                            else
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "2 рази голосувати нізя");
                            return;
                        }


                        currentVoice.Votes.Add(ev.CallbackQuery.From.FirstName + ev.CallbackQuery.From.LastName, num);

                        for (var i = 0; i < variants.Length; i++)
                        {
                            if (i == num)
                                currentVoice.AnswerCounts[i]++;
                            var nums = currentVoice.Votes.Where(a => a.Value == i).ToArray().Length;
                            buttons[i] = new InlineKeyboardCallbackButton(variants[i] + " (" + nums + ")", "callbackVoice" + i);
                        }
                        var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[][] { buttons });
                        try
                        {
                            await Bot.EditMessageTextAsync(message.Chat.Id, currentVoice.MessageId, currentVoice.Question, Telegram.Bot.Types.Enums.ParseMode.Default, false, keyboard);
                        }
                        catch {}
                    }
                }
                catch {}
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

        /// <param name="type">jpg, png or gif</param>
        static async Task<byte[]> GetCat(string type)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetByteArrayAsync("http://thecatapi.com/api/images/get.php?type=" + type);
                return response;
            }
        }

        static async Task<string> GetRandomVideo()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://randomyoutube.net/api/getvid?api_token=8hfjFJWZ1RDANnyZ6d76EeDP6AOVgOT3qBk9vF1KlazNbJ5uT3jf9340kYcc"))
                {
                    using (var content = response.Content)
                    {
                        var myContent = await content.ReadAsStringAsync();
                        var data = (JObject)JsonConvert.DeserializeObject(myContent);
                        var vidID = data["vid"].Value<string>();
                        return "https://www.youtube.com/watch?v=" + (vidID);
                    }
                }
            }
        }

        static async Task<string> GetCalc(string expression)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://newton.now.sh/simplify/" + expression))
                {
                    using (var content = response.Content)
                    {
                        var myContent = await content.ReadAsStringAsync();
                        var data = (JObject)JsonConvert.DeserializeObject(myContent);
                        var result = data["result"].Value<string>();
                        return expression + " = " + result;
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

