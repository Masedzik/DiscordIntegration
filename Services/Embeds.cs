namespace DiscordIntegration.Services;

using Discord;

public static class Embeds
{
    public static Embed CreateEmbed(string command, string response, Color color)
    {
        if (response.Length > 4096) response = response.Substring(0, 4096);
        Embed embed = new EmbedBuilder()
        {
            Title = command,
            Description = response,
            Color = color,
            Footer = new EmbedFooterBuilder(){Text = $"DiscordIntegration | 1.0.0 | Mased • {DateTime.Now}" }
        }.Build();

        return embed;
    }
}