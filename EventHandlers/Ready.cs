namespace DiscordIntegration.EventHandlers;

using System.Reflection;
using Discord.Interactions;
using DiscordIntegration.Services;
using DiscordIntegration.Scp;

public class Ready
{
    private readonly Bot _bot;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;

    public Ready(InteractionService commands, IServiceProvider services, Bot bot)
    {
        _bot = bot;
        _commands = commands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _bot.Client.Ready += OnReady;
    }

    private async Task OnReady()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _commands.RegisterCommandsToGuildAsync(Program.Config.DiscordServerIds[_bot.ServerNumber]);

        Log.Info($"Connected as {_bot.Client.CurrentUser}");
        _bot.Server = new ScpServer(_bot);
    }
}