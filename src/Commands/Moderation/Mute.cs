using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DucksBot.Commands
{
    /// <summary>
    /// This part is responsible for muting, temporarily muting or unmuting a specific user
    /// </summary>
    public partial class Moderation
    {
        [Command("mute")]
        [Description("Mutes a specific user for the specified time and unmutes them after this time has passed.")]
        public async Task MuteCommandAsync(CommandContext ctx, DiscordMember user, string length)
        {
            var role = Utilities.GetRoleByName("Muted", ctx);

            if (!user.Roles.Contains(role))
                await user.GrantRoleAsync(role);

            var span = Utilities.TransformTimeAbbreviation(length);
            if (span is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidParams, ctx);
                return;
            }

            //TemporaryInfraction infraction = new TemporaryInfraction(user, length);
            //InfractionService.Infractions.Add(infraction);
        }
    }
}