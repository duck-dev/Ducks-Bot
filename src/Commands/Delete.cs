﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DucksBot.Commands
{
    /// <summary>
    /// This command will delete the last x messages
    /// or the last x messages of a specific user
    /// author: Duck
    /// </summary>
    public class Delete : BaseCommandModule
    {
        private const int MessageLimit = 50;
        private const string CallbackLimitExceeded = ", since you can't delete more than 50 messages at a time.";

        /// <summary>
        /// Delete the last x messages of any user
        /// </summary>
        [Command("delete")]
        [Aliases("clear", "purge")]
        [Description("Deletes the last x messages in the channel, the command was invoked in (e.g. `\\delete 10`)." +
                     "\nIt contains an overload to delete the last x messages of a specified user (e.g. `\\delete @User 10`)." +
                     "\nThis command can only be invoked by a Helper or Mod.")]
        [RequirePermissions(Permissions.ManageMessages)] // Restrict this command to users/roles who have the "Manage Messages" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict this command to the "Mod" role only
        public async Task DeleteCommand(CommandContext ctx, [Description("How many messages should be deleted?")] int count)
        {
            if (count <= 0)
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidParamsDelete, ctx, count);
                return;
            }

            bool limitExceeded = CheckLimit(count);

            var messages = ctx.Channel.GetMessagesAsync(count + 1).Result;
            await DeleteMessages(ctx, messages);

            await Success(ctx, limitExceeded, count);
        }

        /// <summary>
        /// Delete the last x messages of the specified user
        /// </summary>
        [Command("delete")]
        [RequirePermissions(Permissions.ManageMessages)] // Restrict this command to users/roles who have the "Manage Messages" permission
        [RequireRoles(RoleCheckMode.Any, "Mod")] // Restrict this command to the "Mod" role only
        public async Task DeleteCommand(CommandContext ctx, [Description("Whose last x messages should get deleted?")]DiscordMember targetUser, 
            [Description("How many messages should get deleted?")] int count)
        {
            if (targetUser is null)
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidUser, ctx);
                return;
            }
            if (count <= 0)
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidParamsDelete, ctx, count);
                return;
            }

            bool limitExceeded = CheckLimit(count);

            var allMessages = ctx.Channel.GetMessagesAsync().Result; // Get last 100 messages
            var userMessages = allMessages.Where(x => x.Author == targetUser).Take(count + 1);
            await DeleteMessages(ctx, userMessages);

            await Success(ctx, limitExceeded, count, targetUser);
        }

        /// <summary>
        /// The core-process of deleting the messages
        /// </summary>
        public async Task DeleteMessages(CommandContext ctx, IEnumerable<DiscordMessage> messages)
        {
            foreach (DiscordMessage m in messages)
            {
                if (m != ctx.Message)
                    await m.DeleteAsync();
            }
        }

        /// <summary>
        /// Will be called at the end of every execution of this command and tells the user that the execution succeeded
        /// including a short summary of the command (how many messages, by which user etc.)
        /// </summary>
        private async Task Success(CommandContext ctx, bool limitExceeded, int count, DiscordMember targetUser = null)
        {
            string mentionUserStr = targetUser == null ? string.Empty : $"by '{targetUser.DisplayName}'";
            string overLimitStr = limitExceeded ? CallbackLimitExceeded : string.Empty;
            string messagesLiteral = Utilities.PluralFormatter(count, "message", "messages");
            string hasLiteral = Utilities.PluralFormatter(count, "has", "have");

            await ctx.Message.DeleteAsync();
            string embedMessage = $"The last {count} {messagesLiteral} {mentionUserStr} {hasLiteral} been successfully deleted{overLimitStr}.";

            var message = await Utilities.BuildEmbedAndExecute("Success", embedMessage, Utilities.Green, ctx, true);
            await Task.Delay(10_000);
            await message.DeleteAsync();
        }

        private bool CheckLimit(int count)
        {
            return count > MessageLimit;
        }
    }
}