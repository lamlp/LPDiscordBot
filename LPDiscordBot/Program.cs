using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
namespace LPDiscordBot
{
    class Program
    {
        public static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        public async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            await _client.LoginAsync(TokenType.Bot, ""); // Your Token here

            await _client.StartAsync();

            _handler = new CommandHandler(_client);

            _client.Log += Log;
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
