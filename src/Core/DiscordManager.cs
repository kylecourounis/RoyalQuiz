using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace RoyalQuiz.Core
{
    public class DiscordManager
    {
        public static DiscordSocketClient Client = new DiscordSocketClient(new DiscordSocketConfig
        {
#if DEBUG
            LogLevel = LogSeverity.Debug,
#else
            LogLevel = LogSeverity.Info,
#endif
            DefaultRetryMode = RetryMode.RetryTimeouts
        });

        private static readonly CommandHandler CommandHandler = new CommandHandler();

        public static async void Initialize()
        {
            await CommandHandler.InstallAsync(Client);

            Client.UserJoined += OnUserJoinedGuild;
            Client.MessageReceived += OnMessageReceived;
            Client.Log += OnLog;
            Client.ReactionAdded += OnReactionAdded;

            await Client.LoginAsync(TokenType.Bot, Constants.Token);
            await Client.StartAsync();

            await Client.SetGameAsync("Created By: Kyle & Incredible");
        }

        private static async Task OnUserJoinedGuild(SocketGuildUser socketGuildUser)
        {
            var socketTextChannel = socketGuildUser.Guild.TextChannels.First(x => x.Name == "entrance");

            if (socketTextChannel != null)
                await socketTextChannel.SendMessageAsync(
                    $"Welcome {socketGuildUser.Mention}!\nIf you want to see the other channels please start a short quiz to receive the member role. Enter `!quiz`");
        }

        private static Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage.Author.IsBot) return Task.CompletedTask;

            var quiz = Program.ActiveQuizzes.Find(value => value.QuizTaker.Id.Equals(socketMessage.Author.Id));
            if (quiz == null)
                return Task.CompletedTask;

            if (socketMessage.Channel.Id != quiz.DirectMessage.Id)
                return Task.CompletedTask;

            if (quiz.CorrectAnswer == 0 &&
                (socketMessage.Content.Equals("yes", StringComparison.InvariantCultureIgnoreCase) ||
                 socketMessage.Content.Equals("y", StringComparison.InvariantCultureIgnoreCase)))
                quiz.SendQuestion();

            return Task.CompletedTask;
        }

        private static Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachable, ISocketMessageChannel channel,
            SocketReaction socketReaction)
        {
            if (socketReaction.UserId == Client.CurrentUser.Id) return Task.CompletedTask;
            if (Client.GetUser(socketReaction.UserId).IsBot) return Task.CompletedTask;

            var quiz = Program.ActiveQuizzes.Find(value => value.QuizTaker.Id.Equals(socketReaction.UserId));
            if (quiz == null)
                return Task.CompletedTask;

            if (channel.Id != quiz.DirectMessage.Id)
                return Task.CompletedTask;

            quiz.ProcessAnswer(socketReaction);

            return Task.CompletedTask;
        }

        private static Task OnLog(LogMessage logMessage)
        {
            Logger.Log(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}