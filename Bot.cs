namespace DiscordIntegration;

using System;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordIntegration.EventHandlers;
using DiscordIntegration.Services;
using DiscordIntegration.Scp;

public class Bot
{
    private string _token;
    private DiscordSocketClient? _client;
    private SocketGuild? _guild;
    private static readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences,
        AlwaysDownloadUsers = true,
    };

    public byte ServerNumber { get; }
    public DiscordSocketClient Client => _client ??= new DiscordSocketClient();
    public SocketGuild Guild => _guild ??= Client.GetGuild(Program.Config.DiscordServerIds[ServerNumber]);
    public ScpServer ?Server;

    internal readonly List<KeyValuePair<ulong, string>> Messages = new();

    public Bot(byte port, string token)
    {
        _token = token;
        ServerNumber = port;
        Task.Run(Init);
    }

    private async Task Init()
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, _token);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return;
        };

        using (ServiceProvider services = ConfigureServices())
        {
            _client = services.GetRequiredService<DiscordSocketClient>();

            await services.GetRequiredService<InteractionCreated>().InitializeAsync();
            await services.GetRequiredService<GuildMemberUpdated>().InitializeAsync();
            await services.GetRequiredService<Ready>().InitializeAsync();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            await DequeueMessages();
            await Task.Delay(-1);
        };
    }

    private async Task DequeueMessages()
    {
        for (; ; )
        {
            List<KeyValuePair<ulong, string>> toSend = new();
            lock (Messages)
            {
                foreach (KeyValuePair<ulong, string> message in Messages)
                    toSend.Add(message);

                Messages.Clear();
            }

            foreach (KeyValuePair<ulong, string> message in toSend)
            {
                try
                {
                    await Guild.GetTextChannel(message.Key).SendMessageAsync(message.Value);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            await Task.Delay(1000);
        }
    }

    private ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
        .AddSingleton(this)
        .AddSingleton(_socketConfig)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
        .AddSingleton<InteractionCreated>()
        .AddSingleton<GuildMemberUpdated>()
        .AddSingleton<Ready>()
        .BuildServiceProvider();
    }

    public void Destroy() => Client.LogoutAsync();
}