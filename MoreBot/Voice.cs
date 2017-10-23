using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreBot
{
    public class Voice
    {
        public List<string> Users { get; set; }
        public string Question { get; set; }
        public string[] Answers { get; set; }
        public Dictionary<string, int> Votes { get; set; }
        public int[] AnswerCounts { get; set; }
        public int MessageId { get; set; }
        public bool IsOpen { get; set; }

        public Voice(int mId, string question, string[] answers, bool open = false)
        {
            MessageId = mId;
            Question = question;
            Answers = answers;
            AnswerCounts = new int[answers.Length];
            Users = new List<string>();
            IsOpen = open;
            Votes = new Dictionary<string, int>();
        }
    }
}
