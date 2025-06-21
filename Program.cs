namespace DiscordIntegration;

using System;
using DiscordIntegration.Configs;
using DiscordIntegration.Services;
using DiscordIntegration.Databases;

public static class Program
{
    private static List<Bot> _bots = new();
    public static Config Config => Config.GetConfig();
    public static Roles Roles = Roles.GetRoles();
    public static Users Users = Users.GetUsers();
    public static Translations Translations = Translations.GetTranslations();
    public static Database Database;

    public static void Main(string[] args)
    {
        Log.Info($"Welcome to Discord Integration 1.0.0!");

        foreach (KeyValuePair<byte, string> botToken in Config.BotTokens)
        {
            if (Config.Enabled[botToken.Key]) _bots.Add(new Bot(botToken.Key, botToken.Value));
        };

        if (Config.Database.Enabled)
        {
            Database = new Database(Config.Database.Server, Config.Database.Username, Config.Database.Password, Config.Database.Database, Config.Database.Timeout);
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            foreach (Bot bot in _bots)
            {
                bot.Destroy();
            };
        };

        KeepAlive().GetAwaiter().GetResult();
    }

    private static async Task KeepAlive()
    {
        await Task.Delay(-1);
    }
}