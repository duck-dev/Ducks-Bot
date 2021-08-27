using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DucksBot.Services;

namespace DucksBot.Commands
{
    /// <summary>
    /// This part is responsible for muting, temporarily muting or unmuting a specific user
    /// </summary>
    public partial class Moderation
    {
        [Command("mute")]
        [Description("Mutes a specific user for the specified time and unmutes them after this time has passed.")]
        [RequirePermissions(Permissions.ManageRoles)] // Restrict access to users with "Manage Roles" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict access to "Mod" role
        public async Task MuteCommandAsync(CommandContext ctx, 
            [Description("The user to mute.")] DiscordMember user, 
            [Description("The length of this temporary mute (string will be converted to timespan).")] string length)
        {
            await MuteCommandAsync(ctx, user, length, null);
        }

        [Command("mute")]
        [Description("Mutes a specific user for the specified time and unmutes them after this time has passed.")]
        [RequirePermissions(Permissions.ManageRoles)] // Restrict access to users with "Manage Roles" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict access to the "Mod" role
        public async Task MuteCommandAsync(CommandContext ctx,
            [Description("The user to mute.")] DiscordMember user,
            [Description("The length of this temporary mute (string will be converted to timespan).")] string length,
            [Description("The reason for the mute")] string reason)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            var role = Utilities.GetRoleByName("Muted", ctx);

            if (!user.Roles.Contains(role))
                await user.GrantRoleAsync(role, reason);

            var span = Utilities.TransformTimeAbbreviation(length);
            if (span is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidParams, ctx);
                return;
            }

            TemporaryInfraction infraction = new TemporaryInfraction(InfractionTypes.TempMute, user, span, ctx.Guild);
            InfractionService.Infractions.Add(infraction);
            
            await Utilities.BuildModerationCallback(reason, span.ToString(), user, ctx, InfractionTypes.TempMute);
        }
    }
}