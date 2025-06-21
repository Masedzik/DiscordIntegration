namespace DiscordIntegration.Commands;

using Discord;
using Discord.Interactions;
using DiscordIntegration.Services;

[Group("user", "Users command group.")]
public class UserCommand : InteractionModuleBase<SocketInteractionContext>
{
    private Bot _bot;
    public UserCommand(Bot bot)
    {
        _bot = bot;
    }
    
    [SlashCommand("add", "Adds synchronization to selected user.")]
    public async Task UserAdd(IUser user, string steam)
    {
        try
        {
            if (Patterns.CheckPattern(steam) != true)
            {
                await RespondAsync(embed: Embeds.CreateEmbed("/user add", $"User ID **{steam}** not matching any pattern!", Color.Red));
                return;
            };

            if (Program.Users.List.Values.Contains(steam))
            {
                await RespondAsync(embed: Embeds.CreateEmbed("/user add", $"User ID **{steam}** is already connected with **<@{Program.Users.List.Where(x=> x.Value == steam).ElementAt(0).Key}>**!", Color.Red));
                return;
            };

            if (Program.Users.List.ContainsKey(user.Id))
            {
                Program.Users.List[user.Id] = steam;
                Program.Users.SaveUsers();
                await RespondAsync(embed: Embeds.CreateEmbed("/user add", $"User **{user.Id}** updated to **{steam}**!", Color.Green));
                return;
            };

            Program.Users.List.Add(user.Id, steam);
            Program.Users.SaveUsers();
            await RespondAsync(embed: Embeds.CreateEmbed("/user add", $"User **{user.Id}** connected with **{steam}**!", Color.Green));
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            await RespondAsync(embed: Embeds.CreateEmbed("/user add", $"Unable to synchronize **{user.Id}** with **{steam}**!", Color.Red));
        };
    }

    [SlashCommand("del", "Deletes synchronization with selected user.")]
    public async Task UserDel(IUser user)
    {
        try
        {
            if (!Program.Users.List.ContainsKey(user.Id))
            {
                await RespondAsync(embed: Embeds.CreateEmbed("/user del", $"Discord ID **{user.Id}** is not connected with any ingame ID!", Color.Red));
                return;
            };

            Program.Users.List.Remove(user.Id);
            Program.Users.SaveUsers();
            await RespondAsync(embed: Embeds.CreateEmbed("/user del", $"Removed synchronization of **{user.Id}** !", Color.Green));
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            await RespondAsync(embed: Embeds.CreateEmbed("/user del", $"Unable to remove synchronization of **{user.Id}**", Color.Red));
        };
    }
}