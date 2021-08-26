using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DucksBot.Commands;
using DucksBot.Services;

namespace DucksBot
{
    static class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args[0], (args.Length > 1 && args[1].Length > 0) ? args[1] : "!").GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string token, string prefix)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            
            discord.UseInteractivity(new InteractivityConfiguration() {
                Timeout = TimeSpan.FromHours(2)
            });
            CustomCommandsService.DiscordClient = discord;
            
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
                StringPrefixes = new[] { prefix[0].ToString() } // ! will be the default command prefix if nothing else is specified in the parameters
            });
            commands.CommandErrored += CommandHandlerService.CommandErrorAsync;
            commands.RegisterCommands(Assembly.GetExecutingAssembly()); // Registers all defined commands
            
            await CustomCommandsService.LoadCustomCommandsAsync();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        } 
    }
}