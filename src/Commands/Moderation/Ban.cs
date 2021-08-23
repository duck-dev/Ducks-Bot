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
        public async Task BanCommand(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully banned!";
            await Ban(user, ctx, description);
        }

        [Command("ban")]
        [Description("Bans the specified user with a specified reason.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task BanCommand(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user, 
            [Description("The reason for the ban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await Ban(user, ctx, description, reason);
        }

        [Command("softban")]
        [Description("Bans the specified user, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommand(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully banned!";
            await Ban(user, ctx, description, null, true);
        }
        
        [Command("softban")]
        [Description("Bans the specified user with a specified reason, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommand(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user,
            [Description("The reason for the ban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await Ban(user, ctx, description, reason, true);
        }

        private static async Task Ban(DiscordMember user, CommandContext ctx, string description, string reason = null, bool softban = false)
        {
            if (user is null)
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidUser, ctx);
                return;
            }

            int limit = softban ? 0 : 7;
            try
            {
                await user.BanAsync(limit, reason);
                await Utilities.BuildEmbedAndExecute($"Banned {user.DisplayName}", description, Utilities.Green, ctx, false);
            }
            catch (Exception e)
            {
                await Utilities.ErrorCallback(CommandErrors.UnknownError, ctx);
            }
        }
        
        // <-------------- Temporary ban --------------> //

        // [Command("tempban")]
        // [Description("")]
        // [RequirePermissions(Permissions.BanMembers)]
        // [RequireRoles(RoleCheckMode.Any, "Mod")]
        // public async Task TempbanCommand(CommandContext ctx, DiscordMember user, string length)
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
        public async Task UnbanCommand(CommandContext ctx, 
            [Description("The user to unban.")] DiscordMember user)
        {
            string description = $"{user.DisplayName} has been successfully unbanned!";
            await Unban(user, ctx, description);
        }

        [Command("unban")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task UnbanCommand(CommandContext ctx,
            [Description("The user to unban.")] DiscordMember user,
            [Description("The reason for the unban.")] string reason)
        {
            string description = $"{user.DisplayName} has been successfully banned for the following reason:{Environment.NewLine}**{reason}**";
            await Unban(user, ctx, description, reason);
        }

        private static async Task Unban(DiscordMember user, CommandContext ctx, string description, string reason = null)
        {
            if (user is null)
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidUser, ctx);
                return;
            }

            try
            {
                await user.UnbanAsync(reason);
                await Utilities.BuildEmbedAndExecute($"Unbanned {user.DisplayName}", description, Utilities.Green, ctx, false);
            }
            catch (Exception e)
            {
                await Utilities.ErrorCallback(CommandErrors.UnknownError, ctx);
            }
        }
    }
}