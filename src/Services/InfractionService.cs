using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.Entities;

namespace DucksBot.Services
{
    public static class InfractionService
    {
        internal static List<TemporaryInfraction> Infractions { get; } = new List<TemporaryInfraction>();
        private static Timer timer;

        internal static void Initialize()
        {
            timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += async (sender, args) => await EvaluateInfractionsAsync();
            timer.Start();
        }

        private static async Task EvaluateInfractionsAsync()
        {
            DateTime now = DateTime.Now;
            for (int i = Infractions.Count - 1; i >= 0; i--)
            {
                TemporaryInfraction infr = Infractions[i];
                DiscordMember user = infr.User;
                if (infr.ReleaseTime <= now)
                {
                    Infractions.Remove(infr);
                    switch (infr.InfractionType)
                    {
                        case InfractionTypes.TempBan:
                            await user.UnbanAsync();
                            break;
                        case InfractionTypes.TempMute:
                            var role = Utilities.GetRoleByName("Muted", infr.Guild);
                            await user.RevokeRoleAsync(role);
                            break;
                    }
                }
            }
        }

        public static void RemoveInfractionPrematurely(DiscordMember user, InfractionTypes type)
        {
            var infr = Infractions.FirstOrDefault(x => x.User == user && x.InfractionType == type);
            if (infr != null)
                Infractions.Remove(infr);
        }
    }

    public enum InfractionTypes
    {
        Ban,
        TempBan,
        Kick,
        TempMute
    }
}