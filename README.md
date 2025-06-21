# Discord Integration

Integration bot written in **.NET**, designed to connect an **SCP: Secret Laboratory** game server with a **Discord server**. It enables remote administration, event logging, and role sync between both platforms.

## 🚀 Features

- **Server Status Monitor**
  - Live updates of player count
- **Real-Time Log Forwarding**
  - Join/leave events, deaths, round starts/ends pushed to Discord/MySql Database
- **Remote Admin Commands**
  - Execute game server commands from Discord (e.g. `/ban`, `/broadcast`)
- **Role & Permission Sync** *(optional)*
  - Maps Discord roles to in-game RemoteAdmin permission groups

## 📦 Requirements

- SCP:SL server with RemoteAdmin enabled
- Discord bot token and application with message and slash command permissions
- Linux host system (Tested on Ubuntu)

## ⚙️ Instalation

1. Copy the bot to your destination directory. 
2. Change permissions, e.g., (chmod 500).
3. Run the bot for the first time to generate default config.
4. Configure the bot and run it again.

## ⚙️ Configuration

Edit the `appsettings.json` file:

```json
{
  "Enabled": {
    "1": "IS_ENABLED (true/false)"
  },
  "BotTokens": {
    "1": "YOUR_BOT_TOKEN"
  },
  "DiscordServerIds": {
    "1": "YOUR_GUILD_ID"
  },
  "Channels": {
    "1": {
      "PlayerList": "DISCORD_CHANNEL_ID (0 - Disabled)",
      "ServerLog": "DISCORD_CHANNEL_ID (0 - Disabled)"
    }
  },
  "PlayerListSettings": {
    "1": {
      "Timeout": "TIMEOUT_IN_MILISECONDS",
      "Color": "PLAYER_LIST_BAR_HEX_COLOR"
    }
  },
  "Servers": {
    "1": {
      "Server": "SERVER_IP:SERVER_PORT",
      "Name": "SERVER_NAME",
      "Password": "YOUR_ADMIN_PASSWORD"
    }
  },
  "Patterns": [
    "ACCEPTABLE_USERID_FORMATS",
    "^[0-9]{17}@steam$",
    "^{17,18}$",
    "^*@northwood$"
  ],
  "Database": {
    "Enabled": "IS_ENABLED (true/false)",
    "Timeout": "TIMEOUT_IN_SECONDS",
    "Server": "SERVER_IP",
    "Database": "DATABASE_NAME",
    "Username": "DATABASE_USER",
    "Password": "USER_PASSWORD"
  }
}
