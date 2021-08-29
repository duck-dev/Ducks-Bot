using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DucksBot.Commands
{
    /// <summary>
    /// This command is responsible for showing information about the specified user.
    /// Nickname, Username (with discriminator), ID, Join Date, Creation Date, Roles
    /// </summary>
    public class Whois : BaseCommandModule
    {
        [Command("whois")]
        [Aliases("userinfo", "userinformation")]
        [Description("Get information about a specific user.")]
        public async Task WhoisCommand(CommandContext ctx, 
            [Description("The user to get information about")] DiscordMember member)
        {
            await GetUserInfo(ctx, member);
        }

        [Command("whoami")]
        [Description("Get information about yourself (your own user account).")]
        public async Task WhoAmICommand(CommandContext ctx)
        {
            await GetUserInfo(ctx, ctx.Member);
        }

        private async Task GetUserInfo(CommandContext ctx, DiscordMember member)
        {
            string title = $"Information about {member.DisplayName}#{member.Discriminator}";
            DateTimeOffset joinDate = member.JoinedAt;
            DateTimeOffset createDate = member.CreationTimestamp;
            var embed = Utilities.BuildEmbed(title, null, DiscordColor.Black, member.AvatarUrl);

            if (member.Nickname != null)
                embed.AddField("NickName", member.Nickname, true);
            embed.AddField("Username", $"{member.Username}#{member.Discriminator}", true);
            embed.AddField("ID", member.Id.ToString(), true);
            embed.AddField("Join Date", joinDate.ToString("G"), true);
            embed.AddField("Creation Date", createDate.ToString("G"), true);
            embed.AddField("Roles", string.Join(',', member.Roles.Select(x => x.Mention)));
            
            await Utilities.LogEmbed(embed, ctx.Channel);
        }
    }
}