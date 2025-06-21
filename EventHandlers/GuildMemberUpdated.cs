namespace DiscordIntegration.EventHandlers;

using System.Data;
using Discord;
using Discord.WebSocket;
using SCP_SL_Query_Client.NetworkObjects;
using DiscordIntegration.Services;

public class GuildMemberUpdated
{
    private readonly Bot _bot;

    public GuildMemberUpdated(Bot bot)
    {
        _bot = bot;
    }

    public async Task InitializeAsync()
    {
        _bot.Client.GuildMemberUpdated += OnGuildMemberUpdated;
    }
    private async Task OnGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> before, SocketGuildUser after)
    {
        try
        {
            if (before.Value.Roles == after.Roles) return;

            
            if(!Program.Users.List.TryGetValue(after.Id, out string steam)) return;
            
            var beforeMax = Program.Roles.List[_bot.ServerNumber].Where(i => before.Value.Roles.Select(i => i.Id).Contains(i.Key)).OrderByDescending(i => i.Value.Priority).ToList();
            var afterMax = Program.Roles.List[_bot.ServerNumber].Where(i => after.Roles.Select(i => i.Id).Contains(i.Key)).OrderByDescending(i => i.Value.Priority).ToList();

            if (beforeMax.Count() == 0 && afterMax.Count() == 0)
            {
                return;
            };

            if (beforeMax.Count() > 0 && afterMax.Count() > 0)
            {
                if (beforeMax[0].Key == afterMax[0].Key) return;
            };

            string role = afterMax.Count == 0 ? "-1" : afterMax[0].Value.IngameId;
            _bot.Server.Query.Send($"/pm setgroup {steam} {role}", QueryMessage.QueryContentTypeToServer.Command);
            
        }
        catch (Exception ex) 
        {
            Log.Error(ex.Message);
        }
    }
}