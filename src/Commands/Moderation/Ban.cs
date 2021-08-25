using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DucksBot.Commands
{
    /// <summary>
    /// This part is responsible for banning, temporarily banning or unbanning a specific user from the Discord guild (server)
    /// </summary>
    public partial class Moderation
    {
        [Command("ban")]
        [Description("Bans the specified user.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task BanCommandAsync(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully banned!";
            await BanAsync(user, ctx, description);
        }

        [Command("ban")]
        [Description("Bans the specified user with a specified reason.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task BanCommandAsync(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user, 
            [Description("The reason for the ban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await BanAsync(user, ctx, description, reason);
        }

        [Command("softban")]
        [Description("Bans the specified user, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully banned!";
            await BanAsync(user, ctx, description, null, true);
        }
        
        [Command("softban")]
        [Description("Bans the specified user with a specified reason, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user,
            [Description("The reason for the ban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await BanAsync(user, ctx, description, reason, true);
        }

        private static async Task BanAsync(DiscordMember user, CommandContext ctx, string description, string reason = null, bool softban = false)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }

            int limit = softban ? 0 : 7;
            try
            {
                await user.BanAsync(limit, reason);
                await Utilities.BuildEmbedAndExecuteAsync($"Banned {user.DisplayName}", description, Utilities.Green, ctx, false);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
        
        // <-------------- Temporary ban --------------> //

        // [Command("tempban")]
        // [Description("")]
        // [RequirePermissions(Permissions.BanMembers)]
        // [RequireRoles(RoleCheckMode.Any, "Mod")]
        // public async Task TempbanCommandAsync(CommandContext ctx, DiscordMember user, string length)
        // {
        //     if (user is null)
        //     {
        //         await Utilities.ErrorCallback(CommandErrors.InvalidUser, ctx);
        //         return;
        //     }
        //     
        //     var span = Utilities.TransformTimeAbbreviation(length);
        //     if (span is null)
        //     {
        //         await Utilities.ErrorCallback(CommandErrors.InvalidParams, ctx);
        //         return;
        //     }
        // }
        
        // <-------------- Unban --------------> //

        [Command("unban")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task UnbanCommandAsync(CommandContext ctx, 
            [Description("The user to unban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully unbanned!";
            await UnbanAsync(user, ctx, description);
        }

        [Command("unban")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task UnbanCommandAsync(CommandContext ctx,
            [Description("The user to unban.")] DiscordMember user,
            [Description("The reason for the unban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await UnbanAsync(user, ctx, description, reason);
        }

        private static async Task UnbanAsync(DiscordMember user, CommandContext ctx, string description, string reason = null)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }

            try
            {
                await user.UnbanAsync(reason);
                await Utilities.BuildEmbedAndExecuteAsync($"Unbanned {user.DisplayName}", description, Utilities.Green, ctx, false);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
    }
}