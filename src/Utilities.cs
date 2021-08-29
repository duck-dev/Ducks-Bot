using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DucksBot.Services;

namespace DucksBot
{
    /// <summary>
    /// Utility functions that don't belong to a specific class or a specific command
    /// "General-purpose" function, which can be needed anywhere.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Common colors
        /// </summary>
        public static readonly DiscordColor Red = new DiscordColor("#f50f48");
        public static readonly DiscordColor Green = new DiscordColor("#32a852");
        public static readonly DiscordColor LightBlue = new DiscordColor("#34cceb");
        public static readonly DiscordColor Yellow = new DiscordColor("#f5bc42");

        /// <summary>
        /// Change a string based on the count it's referring to (e.g. "one apple", "two apples")
        /// </summary>
        /// <param name="count">The count, the string is referring to</param>
        /// <param name="singular">The singular version (referring to only one)</param>
        /// <param name="plural">The singular version (referring to more than one)</param>
        /// <returns>The formatted message</returns>
        public static string PluralFormatter(int count, string singular, string plural)
        {
            return count > 1 ? plural : singular;
        }

        /// <summary>
        /// This functions constructs a path in the base directory of the current executable
        /// with a given raw file name and the fileSuffix (file type)
        /// NOTE: The file suffix must contain a period (e.g. ".txt" or ".png")
        /// </summary>
        /// <param name="directoryName">The name of the final folder, in which the file will be saved</param>
        /// <param name="fileNameRaw">The name of the file (without file type)</param>
        /// <param name="fileSuffix">The file-suffix (file-type, e.g. ".txt" or ".png")</param>
        /// <returns>The constructed path</returns>
        public static string ConstructPath(string directoryName, string fileNameRaw, string fileSuffix)
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryName);
            if (!Directory.Exists(Path.Combine(directoryPath)))
                Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, fileNameRaw.Trim().ToLowerInvariant() + fileSuffix);
        }

        /// <summary>
        /// Builds a Discord embed with a given TITLE, DESCRIPTION and COLOR
        /// </summary>
        /// <param name="title">Embed title</param>
        /// <param name="description">Embed description</param>
        /// <param name="color">Embed color</param>
        /// <param name="imageUrl">Optional embed image URL</param>
        /// <returns>The embed template</returns>
        public static DiscordEmbedBuilder BuildEmbed(string title, string description, DiscordColor color, 
            string imageUrl = null)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder
            {
                Title = title,
                Color = color,
                Description = description
            };
            if (imageUrl != null)
                b.Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = imageUrl
                };

            return b;
        }

        /// <summary>
        /// Builds a Discord embed with a given TITLE, DESCRIPTION and COLOR
        /// and SENDS the embed as a message
        /// </summary>
        /// <param name="title">Embed title</param>
        /// <param name="description">Embed description</param>
        /// <param name="color">Embed color</param>
        /// <param name="ctx">CommandContext, required to send a message</param>
        /// <param name="respond">Respond to original message or send an independent message?</param>
        /// <param name="imageUrl">Optional embed image URL</param>
        /// <returns>The message that has been created</returns>
        public static async Task<DiscordMessage> BuildEmbedAndExecuteAsync(string title, string description,
            DiscordColor color, CommandContext ctx, bool respond, string imageUrl = null)
        {
            var embedBuilder = BuildEmbed(title, description, color, imageUrl);
            return await LogEmbed(embedBuilder, ctx, respond);
        }
        
        /// <summary>
        /// Builds a Discord embed with a given TITLE, DESCRIPTION and COLOR
        /// and SENDS the embed as a message
        /// </summary>
        /// <param name="title">Embed title</param>
        /// <param name="description">Embed description</param>
        /// <param name="color">Embed color</param>
        /// <param name="channel">The channel to send the message in</param>
        /// <param name="imageUrl">Optional embed image URL</param>
        /// <returns>The message that has been created</returns>
        public static async Task<DiscordMessage> BuildEmbedAndExecuteAsync(string title, string description, 
            DiscordColor color, DiscordChannel channel, string imageUrl = null)
        {
            var embedBuilder = BuildEmbed(title, description, color, imageUrl);
            return await LogEmbed(embedBuilder, channel);
        }

        /// <summary>
        /// Sends an embed as a message into the relevant channel
        /// </summary>
        /// <param name="builder">Embed builder with the embed template</param>
        /// <param name="ctx">CommandContext, required to send a message</param>
        /// <param name="respond">Respond to original message or send an independent message?</param>
        /// <returns>The message that has been created</returns>
        public static async Task<DiscordMessage> LogEmbed(DiscordEmbedBuilder builder, CommandContext ctx, bool respond)
        {
            if (respond)
                return await ctx.RespondAsync(builder.Build());

            return await ctx.Channel.SendMessageAsync(builder.Build());
        }

        /// <summary>
        /// Sends an embed as a message into the relevant channel
        /// </summary>
        /// <param name="builder">Embed builder with the embed template</param>
        /// <param name="channel">The channel to send the message in</param>
        /// <returns>The message that has been created</returns>
        public static async Task<DiscordMessage> LogEmbed(DiscordEmbedBuilder builder, DiscordChannel channel)
        {
            return await channel.SendMessageAsync(builder.Build());
        }

        /// <summary>
        /// Function called whenever a custom error should be raised
        /// </summary>
        /// <param name="error">CommandErrors value, which identifies the error</param>
        /// <param name="ctx">The CommandContext is needed, so we can send the error message</param>
        /// <param name="additionalParams">Additional parameters that can be printed if necessary</param>
        /// <exception cref="ArgumentException">Invalid parameters passed</exception>
        internal static async Task ErrorCallbackAsync(CommandErrors error, CommandContext ctx, params object[] additionalParams)
        {
            DiscordColor red = Red;
            string message = string.Empty;
            bool respond = false;
            switch (error)
            {
                case CommandErrors.CommandExists:
                    respond = true;
                    if (additionalParams[0] is string name)
                        message = $"There is already a command containing the alias {additionalParams[0]}";
                    else
                        throw new System.ArgumentException("This error type 'CommandErrors.CommandExists' requires a string");
                    break;
                case CommandErrors.UnknownError:
                    message = "Unknown error!";
                    respond = false;
                    break;
                case CommandErrors.InvalidParams:
                    message = "The given parameters are invalid. Enter \\help [commandName] to get help with the usage of the command.";
                    respond = true;
                    break;
                case CommandErrors.InvalidParamsDelete:
                    if (additionalParams[0] is int count)
                        message = $"You can't delete {count} messages. Try to eat {count} apples, does that make sense?";
                    else
                        goto case CommandErrors.InvalidParams;
                    break;
                case CommandErrors.MissingCommandMainName:
                    message = "There is no command with this name! If it's a CC, please don't use an alias, use the original name!";
                    respond = true;
                    break;
                case CommandErrors.MissingCommand:
                    message = "There is no command or alias with the specified name.";
                    respond = true;
                    break;
                case CommandErrors.NoCustomCommands:
                    message = "There are no CC's currently.";
                    break;
                case CommandErrors.InvalidUser:
                    message = "The specified user doesn't seem to be in this server.";
                    break;
            }

            await BuildEmbedAndExecuteAsync("Error", message, red, ctx, respond);
        }

        // Regex for abbreviated time statement
        private static readonly Regex timePattern =
            new Regex(@"^(?:(\d+)y)?(?:(\d+)mo)?(?:(\d+)w)?(?:(\d+)d)?(?:(\d+)h)?(?:(\d+)m)?(?:(\d+)s?)?$");

        /// <summary>
        /// Turns an abbreviated time statement (string, e.g. '4d' for 4 days) into a TimeSpan
        /// </summary>
        /// <param name="content">Abbreviated time statement</param>
        /// <returns>The TimeSpan created from the abbreviated time statement</returns>
        public static TimeSpan? TransformTimeAbbreviation(string content)
        {
            var match = timePattern.Match(content);
            if (!match.Success)
                return null;
            
            var groups = match.Groups;

            TimeSpan result = new TimeSpan(
                ParseAt(1) * 365 + ParseAt(2) * 30 + ParseAt(3) * 7 + ParseAt(4),
                ParseAt(5),
                ParseAt(6),
                ParseAt(7));
            return result;
            
            int ParseAt(int i)
            {
                int.TryParse(groups[i].Value, out int v);
                return v;
            }
        }

        /// <summary>
        /// Get a role by it's name
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <param name="guild">The guild containing this role</param>
        /// <returns>DiscordRole instance representing the role</returns>
        public static DiscordRole GetRoleByName(string roleName, DiscordGuild guild)
        {
            var role = guild.Roles.FirstOrDefault(x => x.Value.Name == roleName).Value;
            if (role is null)
                throw new InvalidOperationException($"The role {roleName} does not exist in the current guild.");
            
            return role;
        }

        /// <inheritdoc cref="BuildModerationCallback(string, string, DiscordMember, CommandContext, InfractionTypes)"/>
        public static async Task<DiscordMessage> BuildModerationCallback(string reason, DiscordMember user, CommandContext ctx,
            InfractionTypes infractionType)
        {
            return await BuildModerationCallback(reason, null, user, ctx, infractionType);
        }

        /// <summary>
        /// Builds a moderation callback when a moderation action (ban, tempban, kick, mute) has been completed successfully.
        /// It prints the reason why this action has been taken and the duration if it's a temporary infraction. 
        /// </summary>
        /// <param name="reason">Reason for the moderation action</param>
        /// <param name="duration">The duration of the temporary infraction</param>
        /// <param name="user">The user against whom the action was performed</param>
        /// <param name="ctx">The CommandContext required for building the embed</param>
        /// <param name="infractionType">The infraction type</param>
        /// <returns>The message that has been created.</returns>
        public static async Task<DiscordMessage> BuildModerationCallback(string reason, string duration, DiscordMember user, 
            CommandContext ctx,
            InfractionTypes infractionType)
        {
            reason ??= "Unspecified";
            string type = infractionType switch
            {
                InfractionTypes.Ban => "Banned",
                InfractionTypes.TempBan => "Tempbanned",
                InfractionTypes.Kick => "Kicked",
                InfractionTypes.TempMute => "Tempmuted",
                _ => string.Empty
            };
            
            string description = $"**Reason:**\n{reason}";
            if(duration != null)
                description += $"\n\n**Duration:**\n{duration}";
            
            return await BuildEmbedAndExecuteAsync($"{type} '{user.DisplayName}'", description, Red, ctx, false);
        }
    }

    /// <summary>
    /// Types of custom errors
    /// </summary>
    public enum CommandErrors
    {
        InvalidParams,
        InvalidParamsDelete,
        CommandExists,
        UnknownError,
        MissingCommandMainName,
        MissingCommand,
        NoCustomCommands,
        InvalidUser
    }
}