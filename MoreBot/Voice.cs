using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoreBot
{
    [XmlRoot("Settings")]
    [XmlInclude(typeof(Voice))]
    public class Settings
    {
        [XmlArray("VoiceList"), XmlArrayItem("Voice")]
        public List<Voice> VoiceList { get; set; }

        public Settings()
        {
            VoiceList = new List<Voice>();
        }
    }

    [XmlType("Voice")]
    public class Voice
    {
        [XmlElement("Users")]
        public List<string> Users { get; set; }
        [XmlElement("Question")]
        public string Question { get; set; }
        [XmlElement("Answers")]
        public string[] Answers { get; set; }
        [XmlElement("Votes")]
        public SerializableDictionary<string, int> Votes { get; set; }
        [XmlElement("AnswerCounts")]
        public int[] AnswerCounts { get; set; }
        [XmlElement("MessageId")]
        public int MessageId { get; set; }
        [XmlElement("IsOpen")]
        public bool IsOpen { get; set; }

        public Voice() { }
        public Voice(int mId, string question, string[] answers, bool open = false)
        {
            MessageId = mId;
            Question = question;
            Answers = answers;
            AnswerCounts = new int[answers.Length];
            Users = new List<string>();
            IsOpen = open;
            Votes = new SerializableDictionary<string, int>();
        }
    }
}
