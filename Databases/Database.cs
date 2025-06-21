namespace DiscordIntegration.Databases;

using System.Timers;
using MySql.Data.MySqlClient;
using DiscordIntegration.Services;

public class Database 
{
    private string _server;
    private string _username;
    private string _password;
    private string _database;

    private Timer _heartbeat;
    private short _timeout;

    public MySqlConnection Connection { get; set; }
    public List<KeyValuePair<byte, string>> Logs { get; set; } = new();

    public Database(string server, string username, string password, string database, short timeout) 
    {
        _server = server;
        _username = username;
        _password = password;
        _database = database;
        _timeout = timeout;

        string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", _server, _database, _username, _password);
        Connection = new MySqlConnection(connstring);

        try
        {
            Connection.Open();
            MySqlCommand users = Connection.CreateCommand();
            users.CommandText = 
                "CREATE TABLE IF NOT EXISTS logs (" +
                "server TINYINT UNSIGNED NOT NULL," +
                "timestamp TIMESTAMP NOT NULL," +
                "log VARCHAR(1024) NOT NULL);";
            users.ExecuteNonQuery();
        }
        catch 
        {
            Log.Error("Unable to connect to database!");
        };

        Heartbeat();
        DequeueMessages();
    }

    private async Task Heartbeat() 
    {
        _heartbeat = new Timer(_timeout * 1000);
        _heartbeat.Elapsed += async (Object? source, ElapsedEventArgs ev) =>
        {
            try
            {
                if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
            }
        };

        _heartbeat.Start();
    }

    private async Task DequeueMessages()
    {
        for (; ; )
        {
            List<KeyValuePair<byte, string>> toSend = new();
            lock (Logs)
            {
                foreach (KeyValuePair<byte, string> log in Logs)
                    toSend.Add(log);

                Logs.Clear();
            }

            foreach (KeyValuePair<byte, string> log in toSend)
            {
                try
                {
                    MySqlCommand packet = Connection.CreateCommand();
                    packet.CommandText = $"INSERT INTO `logs` (`server`,`timestamp`, `log`) VALUES ({log.Key},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{log.Value}')";
                    packet.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            await Task.Delay(1000);
        }
    }
}