using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DucksBot.Extensions;

namespace DucksBot.Services
{
    public static class LogService
    {
        private static DiscordChannel _trackingChannel;
        private const ulong TrackingChannelID = 881482499056881664; 

        internal static async Task DiscordMemberAdded(DiscordClient client, GuildMemberAddEventArgs args)
        {
            _trackingChannel ??= args.Guild.GetChannel(TrackingChannelID);
            await SendTrackingMessageAsync(args.Member, true);
        }

        internal static async Task DiscordMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs args)
        {
            _trackingChannel ??= args.Guild.GetChannel(TrackingChannelID);
            await SendTrackingMessageAsync(args.Member, false);
        }

        private static async Task SendTrackingMessageAsync(DiscordMember member, bool join)
        {
            string action = join ? "joined" : "left";
            var color = join ? Utilities.Green : Utilities.Red;
            var creationDate = member.CreationTimestamp;
            string localTimeZone = TimeZoneInfo.Local.DisplayName.GetFromUntil(1, ")");
            string description = $"{member.DisplayName}#{member.Discriminator} {action} the server!";
            if (join)
                description += $"\n\n**Member account created**\n{creationDate.LocalDateTime} ({localTimeZone})";

            await Utilities.BuildEmbedAndExecuteAsync($"{member.DisplayName} {action}", description, color, 
                _trackingChannel, member.AvatarUrl);
        }
    }
}