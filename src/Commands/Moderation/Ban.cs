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
        [Description("Bans the specified user providing a specified reason.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task BanCommandAsync(CommandContext ctx, 
            [Description("The user to ban.")] DiscordMember user, 
            [Description("The reason for the ban.")] [RemainingText] string reason)
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
        [Description("Bans the specified user providing a specified reason, but doesn't delete their messages.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("The user to ban.")] DiscordMember user,
            [Description("The reason for the ban.")] [RemainingText] string reason)
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
            [Description("The reason for the temp-ban.")] [RemainingText] string reason)
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
                await Utilities.BuildModerationCallback(reason, span, user, ctx, InfractionTypes.TempBan);
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
        [Description("Unban a specific user by their ID.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task UnbanCommandAsync(CommandContext ctx, 
            [Description("The ID of the user to unban.")] ulong userID)
        {
            await UnbanCommandAsync(ctx, userID, null);
        }

        [Command("unban")]
        [Description("Unban a specific user by their ID.")]
        [RequirePermissions(Permissions.BanMembers)]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task UnbanCommandAsync(CommandContext ctx,
            [Description("The ID of the user to unban.")] ulong userID,
            [Description("The reason for the unban.")] [RemainingText] string reason)
        {
            var user = await ctx.Client.GetUserAsync(userID);
            if (user is null)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidUser, ctx);
                return;
            }
            
            try
            {
                await ctx.Guild.UnbanMemberAsync(userID, reason);
                
                InfractionService.RemoveInfractionPrematurely(user, InfractionTypes.TempBan);
                
                reason ??= "Unspecified";
                string description = $"{user.Username} has been successfully unbanned.\n\n**Reason:**\n{reason}";
                await Utilities.BuildEmbedAndExecuteAsync($"Unbanned {user.Username}", description, Utilities.Green, ctx, false);
            }
            catch (Exception)
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.UnknownError, ctx);
            }
        }
    }
}