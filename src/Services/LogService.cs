using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DucksBot.Services
{
    public static class LogService
    {
        private static DiscordChannel trackingChannel;
        private const ulong trackingChannelID = 881482499056881664; 

        public static async Task DiscordMemberAdded(DiscordClient client, GuildMemberAddEventArgs args)
        {
            trackingChannel ??= args.Guild.GetChannel(trackingChannelID);
            await SendTrackingMessageAsync(args.Member, true);
        }

        public static async Task DiscordMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs args)
        {
            trackingChannel ??= args.Guild.GetChannel(trackingChannelID);
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
                trackingChannel, member.AvatarUrl);
        }
    }
}