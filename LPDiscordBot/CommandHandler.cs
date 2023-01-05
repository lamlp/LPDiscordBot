using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using LPDiscordBot.Services;

namespace LPDiscordBot
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;


        public CommandHandler(DiscordSocketClient client)
        {

            _client = client;

            var _audioSerivce = new AudioService();

            var _mir4Service = new Mir4Service();

            _commands = new CommandService();

            _services = new ServiceCollection()
                        .AddSingleton(_client)
                        .AddSingleton(_commands)
                        .AddSingleton(_audioSerivce)
                        .AddSingleton(_mir4Service)
                        .BuildServiceProvider();

            _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += HandleCommandAsync;
            _commands.AddModuleAsync(typeof(Modules.CommandLine), _services);


        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix("lp-", ref argPos, StringComparison.OrdinalIgnoreCase)) //Tiếp đầu ngữ để gọi bot
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }

        }
    }
}
