using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using RoyalQuiz.Core;
using RoyalQuiz.Helpers;

namespace RoyalQuiz.Logic
{
    public class Quiz
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Quiz" /> class.
        /// </summary>
        public Quiz(SocketGuildUser quizTaker)
        {
            QuizTaker = quizTaker;
            QuestionPool = Questions.GetQuestions(Constants.MaxQuestionsPerQuiz);
            TotalQuestions = QuestionPool.Count;
        }

        public int CorrectAnswer { get; set; }
        public IDMChannel DirectMessage { get; set; }
        public Dictionary<string, string[]> QuestionPool { get; set; }
        public SocketGuildUser QuizTaker { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalQuestions { get; set; }

        /// <summary>
        ///     Starts a new <see cref="Quiz" />.
        /// </summary>
        public async Task Start()
        {
            DirectMessage = await QuizTaker.GetOrCreateDMChannelAsync();

            await QuizTaker.SendMessageAsync("**Welcome to the Royal Quiz!**");
            await QuizTaker.SendMessageAsync(
                "On successful completion of this quiz, you will be granted the member rank!\nIf you're ready to begin, type `yes`, and respond with the letter in bold that is attached to the answer.");
        }

        /// <summary>
        ///     Processes the answer.
        /// </summary>
        public async void ProcessAnswer(SocketReaction reaction)
        {
            if (GetEmojiAsNumber(reaction.Emote.ToString()) == CorrectAnswer) TotalCorrect++;

            if (QuestionPool.Count >= 1)
            {
                SendQuestion();
            }
            else
            {
                var score = (int) ((double) TotalCorrect / TotalQuestions * 100);

                if (score > 79)
                {
                    await QuizTaker.SendMessageAsync(
                        $"Congrats! You passed with a {score}% ({TotalCorrect}/{TotalQuestions})!");

                    var role = QuizTaker.Guild.GetRole(Constants.MemberRole);

                    if (role != null)
                        try
                        {
                            await QuizTaker.AddRoleAsync(role);
                        }
                        catch (Exception)
                        {
                            Logger.Log("Couldn't grant the role!", Logger.ErrorLevel.Warning);
                        }
                    else
                        Logger.Log("Member role not found!", Logger.ErrorLevel.Error);
                }
                else
                {
                    await QuizTaker.SendMessageAsync(
                        $"You failed with a {score}% ({TotalCorrect}/{TotalQuestions})");
                }

                Program.ActiveQuizzes.Remove(this);
            }
        }

        /// <summary>
        ///     Sends the question.
        /// </summary>
        public async void SendQuestion()
        {
            var randKey = QuestionPool.Keys.ToArray()[Program.Random.Next(0, QuestionPool.Count)];
            var answers = QuestionPool[randKey];

            answers.Shuffle();

            var idx = 1;
            var description = string.Empty;

            var emotes = new List<IEmote>();

            foreach (var answer in answers)
            {
                var split = answer.Split("::");
                if (bool.Parse(split[1])) CorrectAnswer = idx;

                description += $"**[{idx}]** {split[0]}" + Environment.NewLine;
                emotes.Add(GetNumberAsEmoji(idx++));
            }

            var builder = new EmbedBuilder
            {
                Title = $"**{randKey}**",
                Description = description,
                Color = new Color(66, 138, 245),
                Footer = new EmbedFooterBuilder
                {
                    Text = $"{-QuestionPool.Count + TotalQuestions + 1}/{TotalQuestions}"
                }
            };

            var questionMessage = await QuizTaker.SendMessageAsync(string.Empty, false, builder.Build());
            QuestionPool.Remove(randKey);

            await questionMessage.AddReactionsAsync(emotes.ToArray());
        }

        /// <summary>
        ///     Returns a number as emoji from 1-10
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Emoji GetNumberAsEmoji(int value)
        {
            var unicode = value switch
            {
                1 => "1️⃣",
                2 => "2️⃣",
                3 => "3️⃣",
                4 => "4️⃣",
                5 => "5️⃣",
                6 => "6️⃣",
                7 => "7️⃣",
                8 => "8️⃣",
                9 => "9️⃣",
                10 => "🔟",
                _ => "1️⃣"
            };

            return new Emoji(unicode);
        }

        /// <summary>
        ///     Returns a emoji as number from 1-10
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetEmojiAsNumber(string value)
        {
            return value switch
            {
                "1️⃣" => 1,
                "2️⃣" => 2,
                "3️⃣" => 3,
                "4️⃣" => 4,
                "5️⃣" => 5,
                "6️⃣" => 6,
                "7️⃣" => 7,
                "8️⃣" => 8,
                "9️⃣" => 9,
                "🔟" => 10,
                _ => 1
            };
        }
    }
}
