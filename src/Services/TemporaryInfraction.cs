using System;
using DSharpPlus.Entities;

namespace DucksBot.Services
{
    internal class TemporaryInfraction
    {
        internal InfractionTypes InfractionType { get; private set; }
        internal DiscordMember User { get; private set; }
        internal DateTime ReleaseTime { get; private set; }
        internal DiscordGuild Guild { get; private set; }

        internal TemporaryInfraction(InfractionTypes type, DiscordMember user, TimeSpan? length, DiscordGuild guild)
        {
            this.InfractionType = type;
            this.User = user;
            this.ReleaseTime = DateTime.Now.Add((TimeSpan)length);
            this.Guild = guild;
        } 
    }
}