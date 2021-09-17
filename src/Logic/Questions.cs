using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using RoyalQuiz.Helpers;

namespace RoyalQuiz.Logic
{
    public class Questions
    {
        public static Dictionary<string, string[]> AllQuestions = new Dictionary<string, string[]>();

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Questions" /> is initialized.
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        ///     Initializes the <see cref="Questions" /> class.
        /// </summary>
        public static void Initialize()
        {
            if (Initialized) return;

            foreach (var (key, value) in JObject.Parse(File.ReadAllText("Resources/Questions.json")))
                AllQuestions.Add(key, value.ToObject<string[]>());

            Logger.Log($"Initialized {AllQuestions.Count} questions.");
            Initialized = true;
        }

        public static Dictionary<string, string[]> GetQuestions(int amount)
        { 
            AllQuestions = AllQuestions.Shuffle();

            return AllQuestions.Take(amount).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}