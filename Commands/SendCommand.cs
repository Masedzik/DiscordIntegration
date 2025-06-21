namespace DiscordIntegration.Commands;

using Discord;
using Discord.Interactions;
using DiscordIntegration.Services;

public class SendCommand : InteractionModuleBase<SocketInteractionContext>
{
    private Bot _bot;
    public SendCommand(Bot bot)
    {
        _bot = bot;
    }

    [SlashCommand("send", "Sends command to the server")]
    public async Task Send(string comand)
    {
        _bot.Server.Query.Send(comand, SCP_SL_Query_Client.NetworkObjects.QueryMessage.QueryContentTypeToServer.Command);
        await RespondAsync(embed: Embeds.CreateEmbed("/send", $"Command sended!", Color.Green));
    }
}