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
            var creationDate = member.CreationTimestamp;
            string description = $"{member.DisplayName}{member.Discriminator} {action} the server!" +
                                 $"\n\n**Member account created**\n{creationDate.UtcDateTime}\n({creationDate.LocalDateTime})";
            
            await Utilities.BuildEmbedAndExecuteAsync($"{member.DisplayName} joined", description, Utilities.Green, 
                trackingChannel, member.AvatarUrl);
        }
    }
}