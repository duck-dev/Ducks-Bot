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
            [Description("The reason for the mute")] [RemainingText] string reason)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }

            await MuteUserAsync(user, ctx.Guild, reason);

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

        [Command("unmute")]
        [Description("Unmutes a specific user.")]
        [RequirePermissions(Permissions.ManageRoles)] // Restrict access to users with "Manage Roles" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict access to the "Mod" role
        public async Task UnmuteCommandAsync(CommandContext ctx,
            [Description("The user to unmute.")] DiscordMember user)
        {
            await UnmuteCommandAsync(ctx, user, null);
        }
        
        [Command("unmute")]
        [Description("Unmutes a specific user.")]
        [RequirePermissions(Permissions.ManageRoles)] // Restrict access to users with "Manage Roles" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict access to the "Mod" role
        public async Task UnmuteCommandAsync(CommandContext ctx,
            [Description("The user to unmute.")] DiscordMember user,
            [Description("The reason for the unmute")] [RemainingText] string reason)
        {
            reason ??= "Unspecified";
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            var role = Utilities.GetRoleByName("Muted", ctx.Guild);

            await user.RevokeRoleAsync(role, reason);

            InfractionService.RemoveInfractionPrematurely(user, InfractionTypes.TempMute);
            string description = $"{user.DisplayName} has been successfully unmuted.\n\n**Reason:**\n{reason}";
            await Utilities.BuildEmbedAndExecuteAsync($"Unmuted {user.DisplayName}", description, Utilities.Green, ctx,
                false);
        }

        public static async Task MuteUserAsync(DiscordMember user, DiscordGuild guild, string reason = null)
        {
            var role = Utilities.GetRoleByName("Muted", guild);

            if (!user.Roles.Contains(role))
                await user.GrantRoleAsync(role, reason);
        }
    }
}