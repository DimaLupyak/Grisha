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
using unirest_net.http;
using VoiceRSS_SDK;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace MoreBot
{
	class ChatIds
	{
		public static long itRevolution = -1001147938733;
		public static long moreSquad = -1001144984487;
	}

    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("474545390:AAHU8XYrFNbsFPMpIklVtqk9NSiCmG3-Fjk");
        private static readonly Random random = new Random();
        private static readonly DateTime runTime = DateTime.Now;
        private static readonly StreamWriter file = new StreamWriter("history.txt");
        private static int answerPosibility = 2;
        private static int huiPosibility = 1;
        private static int imagePosibility = 5;
        private static int stickerPosibility = 2;
        private static bool alive = true;
        private static readonly List<Voice> VoiceList = new List<Voice>();
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
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

		private static async void OnText(MessageEventArgs e)
		{
			Console.WriteLine(e.Message.Date + "  |  " + e.Message.Text + " from " + e.Message.From.FirstName + "  " + e.Message.From.LastName);
			file.WriteLine(e.Message.Text + "  |  " + e.Message.Date + " | " + e.Message.From.FirstName + "  " + e.Message.From.LastName + " = " + e.Message.From.Id);
			if (e.Message.Text.ToLower().Contains("удали"))
			{
				int toRemove = int.Parse(e.Message.Text.ToLower().Substring("удали ".Length));
				if (toRemove > 10)
				{
					await Bot.SendTextMessageAsync(e.Message.Chat.Id, "А не слішком дохуя?");
					return;
				}


				var chatId = e.Message.Chat.Id;
				var text = e.Message.Text;
				if (text.Contains("вморе"))
				{
					text = string.Join(" ", text.Split(' ').Where(x => x != "вморе"));
					chatId = ChatIds.moreSquad;
				}
				int startMessageId = e.Message.MessageId - 1;
				int currentMessageId = startMessageId;
				while (toRemove > 0)
				{
					try
					{
						var msg = await Bot.DeleteMessageAsync(chatId, currentMessageId);
						toRemove--;
					}
					catch (Exception){}
					currentMessageId--;
				}
			}
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
						//Bot.SendTextMessageAsync(e.Message.Chat.Id, damn);
						using (var stream = new MemoryStream(GetSpeach(damn).Result))
						{
							stream.Seek(0, SeekOrigin.Begin);
							var x = Bot.SendAudioAsync(e.Message.Chat.Id, new FileToSend("speech.mp3", stream), damn, 10, "Гріша", damn);


							while (x.Status == TaskStatus.WaitingForActivation)
							{
								Thread.Sleep(1000);
							}
						}
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

			else if (e.Message.Text.ToLower().Split(' ').Contains("скажи"))
			{
				var text = string.Join(" ", e.Message.Text.ToLower().Split(' ').Where(x => x != "скажи"));

				long chatId = e.Message.Chat.Id;

				if (text.Contains("вморе"))
				{
					text = string.Join(" ", text.Split(' ').Where(x => x != "вморе"));
					chatId = ChatIds.moreSquad;
				}
				if (text.Contains("вревол"))
				{
					text = string.Join(" ", text.Split(' ').Where(x => x != "вревол"));
					chatId = ChatIds.itRevolution;
				}

				using (var stream = new MemoryStream(GetSpeach(text).Result))
				{
					stream.Seek(0, SeekOrigin.Begin);
					var x = Bot.SendAudioAsync(chatId, new FileToSend("speech.mp3", stream), "", 10, "Гріша", text);


					while (x.Status == TaskStatus.WaitingForActivation)
					{
						Thread.Sleep(1000);
					}
				}
			}
			else if ((e.Message.Text.ToLower().Split(' ').Contains("випадковий") ||
				e.Message.Text.ToLower().Split(' ').Contains("рандомний")) &&
				e.Message.Text.ToLower().Split(' ').Contains("відосік"))
			{
				var text = string.Join(" ", e.Message.Text.ToLower().Split(' ').Where(x => x != "випадковий" && x != "відосік" && x != "рандомний"));
				Bot.SendTextMessageAsync(e.Message.Chat.Id, GetRandomVideo(text).Result);
			}
			else if (e.Message.Text.ToLower().Split(' ').Contains("відосік"))
			{
				var text = string.Join(" ", e.Message.Text.ToLower().Split(' ').Where(x => x != "відосік"));
				Bot.SendTextMessageAsync(e.Message.Chat.Id, GetVideo(text).Result);
			}
			else if (e.Message.Text.ToLower().Contains("погода"))
			{
				Bot.SendTextMessageAsync(e.Message.Chat.Id, GetWeather().Result);
			}
			else if (e.Message.Text.ToLower().EndsWith("триста") || e.Message.Text.ToLower().EndsWith("300"))
			{
				Bot.SendTextMessageAsync(e.Message.Chat.Id, "отсоси у тракториста");
			}
			else if (e.Message.Text.ToLower().EndsWith(" ="))
			{
				Bot.SendTextMessageAsync(
					e.Message.Chat.Id,
					GetCalc(e.Message.Text.ToLower().Substring(0, e.Message.Text.Length - 2)).Result
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

			else if (e.Message.Text.ToLower().Split(' ').Contains("gif"))
			{
				var tag = string.Join(" ", e.Message.Text.ToLower().Split(' ').Where(x => x != "gif"));
				using (var stream = new MemoryStream(GetGif(tag).Result))
				{
					stream.Seek(0, SeekOrigin.Begin);
					var x = Bot.SendDocumentAsync(e.Message.Chat.Id, new FileToSend("gif.gif", stream));


					while (x.Status == TaskStatus.WaitingForActivation)
					{
						Thread.Sleep(1000);
					}
				}
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
				var mssg = Bot.SendTextMessageAsync(e.Message.Chat.Id, "Аве Макс!!!");
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
				var stickerSet3 = Bot.GetStickerSetAsync("BlueRobots");
				var stickerSet4 = Bot.GetStickerSetAsync("Druzhko");
				var stickerSet5 = Bot.GetStickerSetAsync("terebonk_2");
				var stickerSet6 = Bot.GetStickerSetAsync("teadosug");
				var stickers = stickerSet1.Result.Stickers;
				stickers.AddRange(stickerSet3.Result.Stickers);
				stickers.AddRange(stickerSet4.Result.Stickers);
				stickers.AddRange(stickerSet5.Result.Stickers);
				stickers.AddRange(stickerSet6.Result.Stickers);
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
                        catch { }
                    }
                }
                catch { }
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

        static async Task<string> GetWeather()
        {
            var response = Unirest.get("https://simple-weather.p.mashape.com/weather?lat=49.233083&lng=28.468217")
            .header("X-Mashape-Key", "uNOgVxAHRemshRiAMi6C7GQ68qR4p1dEz8jjsn5eMK4qYLUwW1")
            .header("Accept", "text/plain")
            .asString();
            return response.Body;
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


        static async Task<byte[]> GetGif(string tag)
        {
            // These code snippets use an open-source library. http://unirest.io/net
            var response = Unirest.get("https://giphy.p.mashape.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag=" + tag)
            .header("X-Mashape-Key", "uNOgVxAHRemshRiAMi6C7GQ68qR4p1dEz8jjsn5eMK4qYLUwW1")
            .header("Accept", "application/json")
            .asJson<string>();
            var json = (JObject)JsonConvert.DeserializeObject(response.Body);
            var data = json["data"]["fixed_height_downsampled_url"].Value<string>();
            using (var client = new HttpClient())
            {
                var gif = await client.GetByteArrayAsync(data);
                return gif;
            }
        }

        static async Task<string> GetRandomVideo(string key = null)
        {
            if (string.IsNullOrEmpty(key))
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
            else
            {
                return GetVideo(key, 50, true).Result;
            }

        }

        static async Task<string> GetVideo(string key, int count = 50, bool randomVideo = false)
        {
            string videoID;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
              {
                  ApiKey = "AIzaSyDDsOK2xmZrHAI_-DxJziwxbJ2klyZc9Hk",
                  ApplicationName = "Grisha"
              });
            
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = key; // Replace with your search term.
            searchListRequest.MaxResults = count;
            searchListRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;
            searchListRequest.Type = "video";
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();


            if (!randomVideo)
            {
                if (searchListResponse.Items.Any(x => x.Snippet.Title.ToLower().Contains(key)))
                {
                    videoID = searchListResponse.Items.First(x => x.Snippet.Title.ToLower().Contains(key)).Id.VideoId;
                }
                else
                {
                    videoID = searchListResponse.Items[0].Id.VideoId;
                }
            }
            else
            {
                var videos = new List<string>();
                foreach (var searchResult in searchListResponse.Items)
                {
                    videos.Add(searchResult.Id.VideoId);
                }
                videoID =videos[random.Next(videos.Count)];
                
            }

            return "https://www.youtube.com/watch?v=" + videoID;
        }


        static async Task<byte[]> GetSpeach(string text)
        {
            var apiKey = "0d3463c92d324c5599d1fa96e0012a47";
            var isSSL = false;
            var lang = Languages.Russian;

            var voiceParams = new VoiceParameters(text, lang)
            {
                AudioCodec = AudioCodec.MP3,
                AudioFormat = AudioFormat.Format_44KHZ.AF_44khz_16bit_stereo,
                IsBase64 = false,
                IsSsml = false,
                SpeedRate = 0
            };

            var voiceProvider = new VoiceProvider(apiKey, isSSL);
            var voice = await voiceProvider.SpeechTaskAsync<byte[]>(voiceParams);
            return voice;
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

