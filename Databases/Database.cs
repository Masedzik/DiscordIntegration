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
                "CREATE TABLE IF NOT EXISTS logs " +
                    "(server TINYINT UNSIGNED NOT NULL," +
                    "timestamp DATETIME NOT NULL," +
                    "log VARCHAR(1024) NOT NULL," +
                    "INDEX time_index(timestamp)" +
                ")" +
                "PARTITION BY LIST(MONTH(timestamp))(" +
                    "PARTITION p1 VALUES IN(1)," +
                    "PARTITION p2 VALUES IN(2)," +
                    "PARTITION p3 VALUES IN(3)," +
                    "PARTITION p4 VALUES IN(4)," +
                    "PARTITION p5 VALUES IN(5)," +
                    "PARTITION p6 VALUES IN(6)," +
                    "PARTITION p7 VALUES IN(7)," +
                    "PARTITION p8 VALUES IN(8)," +
                    "PARTITION p9 VALUES IN(9)," +
                    "PARTITION p10 VALUES IN(10)," +
                    "PARTITION p11 VALUES IN(11)," +
                    "PARTITION p12 VALUES IN(12)" +
                ");";
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
