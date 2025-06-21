namespace DiscordIntegration.Services;

using System.Text.RegularExpressions;
public static class Patterns
{
    public static bool CheckPattern(string user)
    {
        List<string> patterns = Program.Config.Patterns;
        bool response = false;

        foreach(string pattern in patterns)
        {
            if (Regex.IsMatch(user, pattern) == true) 
            {
                response = true;
            }
        };

        return response;
    }
}