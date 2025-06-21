namespace DiscordIntegration.EventHandlers;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordIntegration.Services;

public class InteractionCreated
{
    private Bot _bot;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;

    public InteractionCreated(InteractionService commands, IServiceProvider services, Bot bot)
    {
        _bot = bot;
        _commands = commands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _bot.Client.InteractionCreated += OnInteractionCreated;
    }

    private async Task OnInteractionCreated(SocketInteraction arg)
    {
        try
        {
            var ctx = new SocketInteractionContext(_bot.Client, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            await arg.RespondAsync(embed: Embeds.CreateEmbed("?", $"Unable to execute command!", Color.Red));
            Log.Error(ex.Message);
        };
    }
}