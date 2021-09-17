using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using RoyalQuiz.Logic;

namespace RoyalQuiz.Modules
{
    [Name("Commands")]
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public static readonly Dictionary<ulong, DateTime> Cooldowns = new Dictionary<ulong, DateTime>();

        private readonly CommandService _cService;

        public Commands(CommandService cService)
        {
            _cService = cService;
        }

        [Command("quiz", RunMode = RunMode.Async)]
        public async Task TakeQuiz(bool skipRoleCheck = false)
        {
            if (Context.Guild == null)
            {
                await ReplyAsync("You can run this command only from a server!");
                return;
            }

            var currentGuildUser = Context.Guild.GetUser(Context.User.Id);
            var hasMemberRole = currentGuildUser.Roles.Any(role => role.Name == "Member");
            if (!skipRoleCheck)
            {
                if (hasMemberRole)
                {
                    Logger.Log($"User {currentGuildUser.Username} already has the member role!",
                        Logger.ErrorLevel.Debug);
                    await ReplyAsync(
                        "You already have the member role! No need for a quiz, if you want to start a quiz anyways just enter `!force` :)");
                    return;
                }
            }

            var currentQuiz = Program.ActiveQuizzes.Find(value => value.QuizTaker.Id.Equals(Context.User.Id));

            if (currentQuiz != null)
            {
                Logger.Log($"User {currentGuildUser.Username} already started a quiz!", Logger.ErrorLevel.Warning);

                await ReplyAsync("You already started a quiz please check your direct messages!");
                return;
            }

            Check:
            if (Cooldowns.ContainsKey(Context.User.Id))
            {
                var cooldown = Cooldowns[Context.User.Id];
                var currentTime = DateTime.UtcNow;

                if (currentTime < cooldown && !hasMemberRole)
                {
                    var timeLeft = cooldown - currentTime;

                    await ReplyAsync(
                        $"Please wait {timeLeft.Hours} hours and {timeLeft.Minutes} minutes before you can take it again.");
                }
                else
                {
                    Cooldowns.Remove(Context.User.Id);
                    goto Check;
                }
            }
            else
            {
                var quiz = new Quiz(currentGuildUser);

                Cooldowns.Add(Context.User.Id, DateTime.UtcNow.AddHours(24));

                Program.ActiveQuizzes.Add(quiz);

                await quiz.Start();
                await ReplyAsync(
                    "Please open your direct messages as we continue the quiz there.");

                Logger.Log($"User {currentGuildUser.Username} started a new quiz.", Logger.ErrorLevel.Debug);
            }
        }

        [Command("force", RunMode = RunMode.Async)]
        public async Task ForceQuiz()
        {
            await TakeQuiz(true);
        }
    }
}