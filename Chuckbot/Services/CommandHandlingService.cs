using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Chuckbot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly SheetsService _sheetsService;
        private readonly IServiceProvider _services;
        private readonly InteractionService _interactionService;

        public CommandHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _sheetsService = services.GetRequiredService<SheetsService>();
            _interactionService = services.GetRequiredService <InteractionService>() ;
            _services = services;
            _discord.SetGameAsync("Now with Slash Commands!");

            _discord.MessageReceived += MessageReceivedAsync;
            _discord.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
            _discord.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task UserVoiceStateUpdatedAsync(SocketUser socketUser, SocketVoiceState fromState, SocketVoiceState toState)
        {
            
        }

        public async Task InitializeAsync()
        {
            var modules = await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interactionService.AddModulesGloballyAsync(true, modules.ToArray());
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            var context = new SocketCommandContext(_discord, message);

            // Ignore system messages, or messages from other bots
            if (message.Source != MessageSource.User) return;

            if (message.Content.ToLower().Contains("gay") || message.Content.ToLower().Contains("no u") || message.Content.ToLower().Contains("no you"))
            {
                await context.Channel.SendMessageAsync("No u");
            }

            if (context.Guild?.Id == 247912266844864513)
            {
                await Birthday(context.Guild);
            }
        }

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            await _interactionService.ExecuteCommandAsync(new SocketInteractionContext<SocketSlashCommand>(_discord, arg), _services);
        }

        public async Task Birthday(IGuild guild)
        {
            var birthdayChannel = await guild.GetTextChannelAsync(1037547995203371181);
            int date = DateTime.Now.Month * 100 + DateTime.Now.Day;
            ulong birthdayMember = 0;
            ulong birthdayMember2 = 0;

            StreamReader sr = new StreamReader("C:\\Discord\\Date2.txt");
            if (sr.ReadLine() != DateTime.Now.ToShortDateString())
            {
                sr.Close();
                string spreadsheetId = "1ND5QhVn8Anw98v07ZxKiAGsGixOanXwCCbJO-4hERe4";
                string range = "'Sheet1'";
                SpreadsheetsResource.ValuesResource.GetRequest request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = await request.ExecuteAsync();
                IList<IList<Object>> values = response.Values;
                foreach (var row in values)
                {
                    if (row.Count > 3 && row[0].ToString() != "Person/Nickname")
                    {
                        if (Convert.ToInt32(row[3]) == date && row[1].ToString() != "")
                        {
                            if (birthdayMember == 0)
                            {
                                birthdayMember = Convert.ToUInt64(row[1]);
                            }
                            else
                            {
                                birthdayMember2 = Convert.ToUInt64(row[1]);
                            }
                        }
                    }
                }
                if (birthdayMember != 0 && birthdayMember2 == 0)
                {
                    await birthdayChannel.SendMessageAsync("Happy Birthday to <@" + birthdayMember + ">!");
                }
                if (birthdayMember != 0 && birthdayMember2 != 0)
                {
                    await birthdayChannel.SendMessageAsync("Happy Birthday to <@" + birthdayMember + "> and <@" + birthdayMember2 + ">!");
                }
                StreamWriter sw = new StreamWriter("C:\\Discord\\Date2.txt");
                sw.WriteLine(DateTime.Now.ToShortDateString());
                sw.Close();
            }
            else
            {
                sr.Close();
            }
        }
    }
}
