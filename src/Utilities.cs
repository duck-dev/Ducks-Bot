using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

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
        public static DiscordEmbedBuilder BuildEmbed(string title, string description, DiscordColor color)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.Title = title;
            b.Color = color;
            b.Description = description;

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
        public static async Task<DiscordMessage> BuildEmbedAndExecute(string title, string description,
            DiscordColor color,
            CommandContext ctx, bool respond)
        {
            var embedBuilder = BuildEmbed(title, description, color);
            return await LogEmbed(embedBuilder, ctx, respond);
        }

        /// <summary>
        /// Logs an embed as a message in the relevant channel
        /// </summary>
        /// <param name="builder">Embed builder with the embed template</param>
        /// <param name="ctx">CommandContext, required to send a message</param>
        /// <param name="respond">Respond to original message or send an independent message?</param>
        public static async Task<DiscordMessage> LogEmbed(DiscordEmbedBuilder builder, CommandContext ctx, bool respond)
        {
            if (respond)
                return await ctx.RespondAsync(builder.Build());

            return await ctx.Channel.SendMessageAsync(builder.Build());
        }
        
        internal static async Task ErrorCallback(CommandErrors error, CommandContext ctx, params object[] additionalParams)
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
                case CommandErrors.MissingCommand:
                    message = "There is no command with this name! If it's a CC, please don't use an alias, use the original name!";
                    respond = true;
                    break;
                case CommandErrors.NoCustomCommands:
                    message = "There are no CC's currently.";
                    break;
            }
        
            await BuildEmbedAndExecute("Error", message, red, ctx, respond);
        }
    }
    
    public enum CommandErrors
    {
        InvalidParams,
        InvalidParamsDelete,
        CommandExists,
        UnknownError,
        MissingCommand,
        NoCustomCommands
    }
}