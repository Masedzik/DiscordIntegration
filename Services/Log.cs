namespace DiscordIntegration.Services;

public static class Log
{
    private static string path = Path.Combine(Environment.CurrentDirectory, @"logs", $"log-{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
    private static void Check()
    {
        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"logs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"logs"));
        };

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        };
    }

    public static void Info(string message)
    {
        Check();
        File.AppendAllText(path, $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][INFO] {message}" + Environment.NewLine);
        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][INFO] {message}");
    }

    public static void Error(string message)
    {
        Check();
        File.AppendAllText(path, $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][ERROR] {message}" + Environment.NewLine);
        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][ERROR] {message}");
    }
}