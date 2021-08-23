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
        public async Task PollCommand(CommandContext ctx, 
            [Description("The entire content of the poll, including question and answers.")] [RemainingText] string content)
        {
            string question = Utilities.GetFromUntil(content, "q:", "a:").Trim();
            string rest = Utilities.GetFromUntil(content, "a:").Trim();
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(rest))
            {
                await Utilities.ErrorCallback(CommandErrors.InvalidParams, ctx);
                return;
            }

            string[] allEmojisStr = rest.Split(',');
            string[] allInserts = allEmojisStr.Select(x => Utilities.GetFromUntil(x, @"""".Trim())).ToArray();
            allEmojisStr = allEmojisStr.Select(x => Utilities.GetUntilOrEmpty(x, @"""".Trim())).ToArray();
            var emojis = new DiscordEmoji[allEmojisStr.Length];

            string description = question;
            for (int i = 0; i < allEmojisStr.Length; i++)
            {
                if (DiscordEmoji.TryFromName(ctx.Client, allEmojisStr[i], out DiscordEmoji emoji))
                {
                    emojis[i] = emoji;
                    description += $"\n{emoji} {allInserts[i]}";
                }
            }

            await ctx.Message.DeleteAsync();
            var message = await Utilities.BuildEmbedAndExecute("Poll", description, Utilities.LightBlue, ctx, false);
            foreach (var e in emojis)
            {
                await message.CreateReactionAsync(e);
            }
        }
    }
}