using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.Entities;

namespace DucksBot.Services
{
    public static class InfractionService
    {
        internal static List<TemporaryInfraction> Infractions { get; } = new List<TemporaryInfraction>();
        private static DiscordGuild currentGuild;
        private static Timer timer;

        internal static void Initialize(DiscordGuild guild)
        {
            currentGuild = guild;
            
            timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += async (sender, args) => await EvaluateInfractions();
            timer.Start();
        }

        private static async Task EvaluateInfractions()
        {
            DateTime now = DateTime.Now;
            foreach (var infr in Infractions)
            {
                DiscordMember user = infr.User;
                if (infr.ReleaseTime <= now)
                {
                    Infractions.Remove(infr);
                    switch (infr.InfractionType)
                    {
                        case InfractionTypes.TempBan:
                            await infr.User.UnbanAsync();
                            break;
                        case InfractionTypes.TempMute:
                            await infr.User.RevokeRoleAsync(Utilities.GetRoleByName("Muted", currentGuild));
                            break;
                    }
                }
            }
        }
    }

    internal enum InfractionTypes
    {
        TempBan,
        TempMute
    }
}