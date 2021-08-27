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
            await BanAsync(user, ctx);
        }

        [Command("ban")]
        [Description("Bans the specified user with a specified reason.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task BanCommandAsync(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user, 
            [Description("The reason for the ban.")] string reason)
        {
            await BanAsync(user, ctx, reason);
        }

        [Command("softban")]
        [Description("Bans the specified user, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user)
        {
            await BanAsync(user, ctx, null, true);
        }
        
        [Command("softban")]
        [Description("Bans the specified user with a specified reason, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user,
            [Description("The reason for the ban.")] string reason)
        {
            await BanAsync(user, ctx, reason, true);
        }

        private static async Task BanAsync(DiscordMember user, CommandContext ctx, string reason = null, bool softban = false)
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
                await Utilities.BuildModerationCallback(reason, user, ctx, InfractionTypes.Ban);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
        
        // <-------------- Temporary ban --------------> //
        
        [Command("tempban")]
        [Description("Bans a user temporarily and unbans them after this certain timespan has passed.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task TempbanCommandAsync(CommandContext ctx, 
            [Description("The user to be temp-banned.")] DiscordMember user, 
            [Description("The length of this temporary mute (string will be converted to timespan).")] string length)
        {
            await TempbanCommandAsync(ctx, user, length, null);
        }

        [Command("tempban")]
        [Description("Bans a user temporarily and unbans them after this certain timespan has passed.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task TempbanCommandAsync(CommandContext ctx, 
            [Description("The user to be temp-banned.")] DiscordMember user, 
            [Description("The length of this temporary mute (string will be converted to timespan).")] string length,
            [Description("The reason for the temp-ban.")] string reason)
        {
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            var span = Utilities.TransformTimeAbbreviation(length);
            if (span is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidParams, ctx);
                return;
            }
            
            try
            {
                await user.BanAsync(7, reason);
                await Utilities.BuildModerationCallback(reason, span.ToString(), user, ctx, InfractionTypes.TempBan);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
            
            TemporaryInfraction infraction = new TemporaryInfraction(InfractionTypes.TempBan, user, span, ctx.Guild);
            InfractionService.Infractions.Add(infraction);
        }
        
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