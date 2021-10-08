using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DucksBot.Commands
{
    /// <summary>
    /// Miscellaneous commands, not specifically dedicated to a specific thing
    /// </summary>
    public class MiscellaneousCommands : BaseCommandModule
    {
        private readonly Random _random = new Random();
        
        [Command("random")]
        [Description("Get a random number in the specified range (both values INCLUSIVE)")]
        public async Task RandomCommand(CommandContext ctx, 
            [Description("Min value")] int a, 
            [Description("Max value")] int b)
        {
            if (a > b)
            {
                await Utilities.BuildEmbedAndExecuteAsync("Error", 
                    "The first value should be smaller than or equal to the second number", Utilities.Red, ctx.Channel);
                return;
            }
            
            var num = _random.Next(a, b + 1);
            await ctx.RespondAsync(num.ToString());
        }
    }
}