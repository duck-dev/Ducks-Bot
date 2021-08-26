using System;
using DSharpPlus.Entities;

namespace DucksBot.Services
{
    internal class TemporaryInfraction
    {
        internal InfractionTypes InfractionType { get; private set; }
        internal DiscordMember User { get; private set; }
        internal DateTime ReleaseTime { get; private set; }

        internal TemporaryInfraction(InfractionTypes type, DiscordMember user, TimeSpan? length)
        {
            this.InfractionType = type;
            this.User = user;
            this.ReleaseTime = DateTime.UtcNow.Add((TimeSpan)length);
        } 
    }
}