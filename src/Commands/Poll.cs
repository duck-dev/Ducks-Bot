using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DucksBot.Commands
{
    /// <summary>
    /// Create a poll with a question and several possible answers. You can answer by reacting with one or more of the given emojis
    /// </summary>
    public class Poll : BaseCommandModule
    {
        [Command("poll")]
        [Description("Create a poll with a question and several possible answers. You can answer by reacting with" +
                     "one of the given emojis.\n\n**Usage:**\n!poll q: Question a: :emoji-1: Answer1, :emoji-2: Answer2")]
        [RequireRoles(RoleCheckMode.Any, "Mod")]
        public async Task PollCommandAsync(CommandContext ctx, 
            [Description("The entire content of the poll, including question and answers.")] [RemainingText] string content)
        {
            string question = content.GetFromUntil("q:", "a:").Trim();
            string rest = content.GetFromUntil("a:").Trim();
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(rest))
            {
                await Utilities.ErrorCallbackAsync(CommandErrors.InvalidParams, ctx);
                return;
            }

            string[] allEmojisStr = rest.Split(',');
            string[] allInserts = allEmojisStr.Select(x => x.GetFromUntil(@"""", x.Length - 1)).ToArray();
            allEmojisStr = allEmojisStr.Select(x => x.GetUntilOrEmpty(@"""")).ToArray();
            var emojis = new DiscordEmoji[allEmojisStr.Length];

            string description = question;

            for (int i = 0; i < allEmojisStr.Length; i++)
            {
                string currentEmoji = allEmojisStr[i];
                currentEmoji = currentEmoji.Trim();

                string currentEmoteWithoutName = currentEmoji;
                if (currentEmoji.Contains(':'))
                {
                    int idBegins = currentEmoji.LastIndexOf(':') + 1;
                    currentEmoteWithoutName = currentEmoji.Substring(idBegins, currentEmoji.Length - idBegins - 1);
                }

                if (DiscordEmoji.TryFromUnicode(currentEmoji, out DiscordEmoji emoji))
                {
                    emojis[i] = emoji;
                    description += $"\n{emoji} {allInserts[i]}";
                } else if (ulong.TryParse(currentEmoteWithoutName, out ulong emoteID) 
                           && DiscordEmoji.TryFromGuildEmote(ctx.Client, emoteID, out DiscordEmoji guildEmote))
                {
                    emojis[i] = guildEmote;
                    description += $"\n{guildEmote} {allInserts[i]}";
                }
            }

            await ctx.Message.DeleteAsync();
            var message = await Utilities.BuildEmbedAndExecuteAsync("Poll", description, Utilities.LightBlue, ctx, false);
            foreach (var e in emojis)
            {
                await message.CreateReactionAsync(e);
            }
        }
    }
}