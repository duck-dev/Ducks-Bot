using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace Ducks_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args[0], (args.Length > 1 && args[1].Length > 0) ? args[1] : "!").GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string token, string prefix)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers
            });
            
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
                StringPrefixes = new[] { prefix[0].ToString() } // ! will be the default command prefix if nothing else is specified in the parameters
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly()); // Registers all defined commands

            await discord.ConnectAsync();
            await Task.Delay(-1);
        } 
    }
}