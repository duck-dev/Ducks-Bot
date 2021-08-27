using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DucksBot.Services;

namespace DucksBot.Commands
{
    /// <summary>
    /// This part is responsible for kicking a user from the Discord guild (server)
    /// </summary>
    public partial class Moderation
    {
        [Command("kick")]
        [Description("Kicks the specified user.")]
        [RequirePermissions(Permissions.KickMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task KickCommandAsync(CommandContext ctx, 
            [Description("User to kick.")] DiscordMember user)
        {
            await KickAsync(user, ctx);
        }

        [Command("kick")]
        [Description("Kicks the specified user with a specified reason.")]
        [RequirePermissions(Permissions.KickMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task KickCommandAsync(CommandContext ctx, 
            [Description("User to kick.")] DiscordMember user, 
            [Description("Reason for the kick.")] [RemainingText] string reason)
        {
            await KickAsync(user, ctx, reason);
        }

        private static async Task KickAsync(DiscordMember user, CommandContext ctx, string reason = null)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            try
            {
                await user.RemoveAsync(reason);
                await Utilities.BuildModerationCallback(reason, user, ctx, InfractionTypes.Kick);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
    }
}