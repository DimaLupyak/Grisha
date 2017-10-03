using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreBot
{
    public enum User
    {
        Dudko   = 359430821,
        Mudra   = 328977113,
        Max     = 380205662,
        Vadim   = 255402619,
        Gedz    = 118055386,
        Yarik   = 288950149,
        Lupiak  = 422335882,
        Toha    = 442395247,
        Mayska  = 252838140,
        Any,
        Image
    }

    public class Phrase
    {
        public User UserId { get; set; }
        public string Text { get; set; }
    }

    public static class Constants
    {
        public static long MoreChatId = -1001144984487;

        public static List<Phrase> Phrases = new List<Phrase>()
        {
            new Phrase {UserId = User.Dudko, Text = @"Скажеш це свому ананасу"},
            new Phrase {UserId = User.Dudko, Text = @"Славік, діставай!"},
            new Phrase {UserId = User.Mudra, Text = @"Ти що, сама мудра тут?"},
            new Phrase {UserId = User.Max,   Text = @"Макс, спасіба, що живий!"},
            new Phrase {UserId = User.Vadim, Text = @"Чувак, класна попа!"},
            new Phrase {UserId = User.Vadim, Text = @"Забей!"},
            new Phrase {UserId = User.Gedz, Text = @"Їбало треба мати попроще!"},
            new Phrase {UserId = User.Gedz, Text = @"Огооо яка машина!!!"},
            new Phrase {UserId = User.Yarik, Text = @"Мені здається, чи фронтендом засмерділо?"},
            new Phrase {UserId = User.Lupiak, Text = @"Нахріна ти створив мене?"},
            new Phrase {UserId = User.Any, Text = @"Оце ти зря так"},
            new Phrase {UserId = User.Any, Text = @"Цілком погоджуюсь"},
            new Phrase {UserId = User.Any, Text = @"Забери свої слова назад"},
            new Phrase {UserId = User.Any, Text = @"Я краще й не сказав би"},
            new Phrase {UserId = User.Any, Text = @"кайф"},
            new Phrase {UserId = User.Any, Text = @"ви знову тут флудите? гріші це подобається"},
            new Phrase {UserId = User.Toha, Text = @"Тоха - гівна ліпьоха"},
            new Phrase {UserId = User.Image, Text = @"найс картінка"},
            new Phrase {UserId = User.Image, Text = @"Щось це не похоже на годний мемас"},
            new Phrase {UserId = User.Image, Text = @"вау, наче богом намальовано"},
            new Phrase {UserId = User.Image, Text = @"зря ти це кинув"},
            new Phrase {UserId = User.Image, Text = @"макс задизайнив би краще"}
        };
}


}
