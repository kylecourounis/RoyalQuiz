using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace RoyalQuiz.Core
{
    public class CommandHandler
    {
        public CommandService CommandService { get; set; }
        public DiscordSocketClient DiscordClient { get; set; }
        public IServiceProvider Services { get; set; }

        public async Task InstallAsync(DiscordSocketClient discordClient)
        {
            DiscordClient = discordClient;
            CommandService = new CommandService();

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);

            DiscordClient.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
                return;

            var context = new SocketCommandContext(DiscordClient, message);

            var argPos = 0;
            if (message.HasStringPrefix(Constants.BotPrefix, ref argPos) ||
                message.HasMentionPrefix(DiscordClient.CurrentUser, ref argPos))
                await CommandService.ExecuteAsync(context, argPos, Services);
        }
    }
}