using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DucksBot.Commands;

namespace DucksBot.Services
{
    public static class CommandHandlerService
    {
        internal static async Task CommandErrorAsync(CommandsNextExtension extension, CommandErrorEventArgs args)
        {
            if (args.Exception is DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException)
            {
                string commandName = args.Context.Message.Content.Split(' ')[0].Substring(1).ToLowerInvariant();
                if (CustomCommandsService.TryGetCommand(commandName, out CustomCommand command))
                    await command.ExecuteCommandAsync(args.Context);
                else
                    await Utilities.ErrorCallbackAsync(CommandErrors.MissingCommand, args.Context);
            }
        }
    }
}