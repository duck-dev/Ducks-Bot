using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

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
            string description = $"{user.DisplayName} has been successfully banned!";
            await KickAsync(user, ctx, description);
        }

        [Command("kick")]
        [Description("Kicks the specified user with a specified reason.")]
        [RequirePermissions(Permissions.KickMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task KickCommandAsync(CommandContext ctx, 
            [Description("User to kick.")] DiscordMember user, 
            [Description("Reason for the kick.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully kicked for the following reason:{Environment.NewLine}**{reason}**";
            await KickAsync(user, ctx, description, reason);
        }

        private static async Task KickAsync(DiscordMember user, CommandContext ctx, string description, string reason = null)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            try
            {
                await user.RemoveAsync(reason);
                await Utilities.BuildEmbedAndExecuteAsync($"Kicked {user.DisplayName}", description, Utilities.Green, ctx, false);
            }
            catch (Exception e)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
    }
}