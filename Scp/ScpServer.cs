namespace DiscordIntegration.Scp;

using System.Text.RegularExpressions;
using Discord;
using SCP_SL_Query_Client;
using SCP_SL_Query_Client.NetworkObjects;
using DiscordIntegration.Services;

public class ScpServer
{
    public QueryClient Query;
    public List<string> Users = new List<string>(0);

    private Bot _bot;
    private string _ip;
    private short _port;

    private System.Timers.Timer _interval;
    private IMessageChannel _channel;
    private IUserMessage? _message;

    public ScpServer(Bot bot)
    {
        _bot = bot;
        _ip = Program.Config.Servers[_bot.ServerNumber].Server.Split(':')[0];
        _port = short.Parse(Program.Config.Servers[_bot.ServerNumber].Server.Split(":")[1]);

        Init();
    }

    private void Init() 
    {
        if (Program.Config.Channels[_bot.ServerNumber].PlayerList != 0)
        {
            try
            {
                _channel = _bot.Guild.GetTextChannel(Program.Config.Channels[_bot.ServerNumber].PlayerList);
                _message = _channel.GetMessagesAsync(255).FlattenAsync().Result.Where(_x => _x.Author.Id == _bot.Client.CurrentUser.Id).ElementAt(0) as IUserMessage;
            }
            catch (Exception ex)
            {
                _message = null;
                Log.Info("Creating status message!");
            };
        };

        _interval = new System.Timers.Timer(Program.Config.PlayerListSettings[_bot.ServerNumber].Timeout);
        _interval.Elapsed += (Object? source, System.Timers.ElapsedEventArgs ev) =>
        {
            if (!Query.Connected) Connect();
            try
            {
                Query.Send("list", QueryMessage.QueryContentTypeToServer.Command);
                if (Program.Config.Channels[_bot.ServerNumber].ServerLog != 0) Query.Send("/subscribe log", QueryMessage.QueryContentTypeToServer.Command);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            };
        };

        _interval.Start();
        Connect();
    }

    private void Connect()
    {
        Query = new QueryClient(_ip, _port, Program.Config.Servers[_bot.ServerNumber].Password);
        Query.OnConnectedToServer += OnConnected;
        Query.OnDisconnectedFromServer += OnDisconnected;
        Query.OnMessageReceived += OnMessage;

        Query.Connect();
    }

    private void OnConnected()
    {
        Query.Send("list",QueryMessage.QueryContentTypeToServer.Command);
        if (Program.Config.Channels[_bot.ServerNumber].ServerLog != 0) Query.Send("/subscribe log", QueryMessage.QueryContentTypeToServer.Command);
    }

    private void OnDisconnected(DisconnectionReason reason)
    {

        _bot.Client.SetCustomStatusAsync(Program.Translations.List["ServerStatusDisconnected"]
            .Replace("{name}", Program.Config.Servers[_bot.ServerNumber].Name)
            .Replace("{users}", Users.Count().ToString()));
        _bot.Client.SetStatusAsync(UserStatus.DoNotDisturb);

        Update();
    }

    private async void OnMessage(QueryMessage message)
    {
        if (Regex.Match(message.ToString(), @"^[0-9]{4}-[0-9]{2}-[0-9]{2} ").Success)
        {
            string log = (Regex.Replace(message.ToString(), @"(?:.*?(\| )+){2}.*?((\| )+)", ""));
            _bot.Messages.Add(new KeyValuePair<ulong, string>(Program.Config.Channels[_bot.ServerNumber].ServerLog, log));
            if(Program.Config.Database.Enabled) Program.Database.Logs.Add(new KeyValuePair<byte, string>(_bot.ServerNumber, log));
            return;
        };

        if (Regex.Match(message.ToString(), "<color=cyan>List of players").Success)
        {
            Users.Clear();
            foreach (Match match in Regex.Matches(message.ToString(), @"^- (.)*: ", RegexOptions.Multiline)) 
            { 
                Users.Add($"- {Regex.Replace(match.Value, @"^- |: $", "`")}");
            };

            await _bot.Client.SetCustomStatusAsync(Program.Translations.List[(Users.Count > 0 ? "ServerStatusConnected" : "ServerStatusEmpty")]
                .Replace("{name}", Program.Config.Servers[_bot.ServerNumber].Name)
                .Replace("{users}", Users.Count().ToString()));
            await _bot.Client.SetStatusAsync(Users.Count > 0 ? UserStatus.Online : UserStatus.AFK);

            Update();
            return;
        };
    }

    private void Update() 
    {
        if (Program.Config.Channels[_bot.ServerNumber].PlayerList == 0) return;

        Embed embed = new EmbedBuilder()
            .WithFields(new EmbedFieldBuilder()
                .WithName(Program.Translations.List["UserListServerName"]
                    .Replace("{name}", Program.Config.Servers[_bot.ServerNumber].Name)
                    .Replace("{users}", Users.Count().ToString()))
                .WithValue(Users.Count() > 0 ? String.Join("\n", Users) : Program.Translations.List["UserListEmpty"])
                .WithIsInline(false))
            .WithColor(Color.Parse(Program.Config.PlayerListSettings[_bot.ServerNumber].Color))
            .WithFooter(new EmbedFooterBuilder()
                .WithText(DateTime.Now.ToShortTimeString().ToString())).Build();

        if (_message == null)
        {
            _channel.SendMessageAsync(embed: embed);
            _message = _channel.GetMessagesAsync(255).FlattenAsync().Result.Where(x => x.Author.Id == _bot.Client.CurrentUser.Id).ToList()[0] as IUserMessage;

            return;
        };

        _message.ModifyAsync(x => x.Embeds = new Embed[] { embed });
    }
}
