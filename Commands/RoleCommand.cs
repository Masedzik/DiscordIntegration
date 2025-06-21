namespace DiscordIntegration.Commands;

using Discord;
using Discord.Interactions;
using DiscordIntegration;
using DiscordIntegration.Services;
using DiscordIntegration.Databases;
using System.Text;

[Group("role", "Role command group.")]
public class RoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private Bot _bot;
    public RoleCommand(Bot bot)
    {
        _bot = bot;
    }
    
    [SlashCommand("add", "Adds synchronization to selected role.")]
    public async Task RoleAdd(IRole role, string name, [MinValue(0)] [MaxValue(255)] int priority)
    {
        try
        {
            if (!Program.Roles.List.ContainsKey(_bot.ServerNumber)) Program.Roles.List.Add(_bot.ServerNumber, new Dictionary<ulong, Role>());

            if (Program.Roles.List[_bot.ServerNumber].ContainsKey(role.Id))
            {
                await RespondAsync(embed: Embeds.CreateEmbed("/role add", $"Role **{role.Mention}** is already synchronized!", Color.Red));
                return;
            };

            Program.Roles.List[_bot.ServerNumber].Add(role.Id, new Role { IngameId = name, Priority = (byte)priority });
            Program.Roles.SaveRoles();
            await RespondAsync(embed: Embeds.CreateEmbed($"/role add", $"Role **{role.Mention}** synchronized with **{name}**.", Color.Green));
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);  
            await RespondAsync(embed: Embeds.CreateEmbed($"/role add", $"Unable to synchronize **{role.Mention}** with **{name}**!", Color.Red));
        }
    }

    [SlashCommand("del", "Deletes synchronization with selected role.")]
    public async Task RoleDel(IRole role)
    {
        try
        {
            if (!Program.Roles.List.ContainsKey(_bot.ServerNumber)) Program.Roles.List.Add(_bot.ServerNumber, new Dictionary<ulong, Role>());

            if (!Program.Roles.List[_bot.ServerNumber].ContainsKey(role.Id))
            {
                await RespondAsync(embed: Embeds.CreateEmbed($"/role del", $"Role **{role.Mention}** is not synchronized!", Color.Red));
                return;
            };

            Program.Roles.List[_bot.ServerNumber].Remove(role.Id);
            Program.Roles.SaveRoles();
            await RespondAsync(embed: Embeds.CreateEmbed($"/role del", $"Removed synchronization with **{role.Mention}**!", Color.Green));
        }
        catch
        {
            await RespondAsync(embed: Embeds.CreateEmbed($"/role del", $"Unable to remove synchronization with ** {role.Mention}** !", Color.Red));
        }
    }

    [SlashCommand("list", "Return list of synchronized roles.")]
    public async Task RoleList()
    {
        try
        {
            StringBuilder list = new StringBuilder();
            foreach (ulong role in Program.Roles.List[_bot.ServerNumber].Keys) 
            {
                list.AppendLine($"- <@&{role}> \n");
            };

            await RespondAsync(embed: Embeds.CreateEmbed($"/role list", list.ToString(), Color.Green));
        }
        catch
        {
            await RespondAsync(embed: Embeds.CreateEmbed($"/role list", $"Unable to show list!", Color.Red));
        }
    }
}