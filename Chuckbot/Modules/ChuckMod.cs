using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Google.Apis.Sheets.v4;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chuckbot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class ChuckMod : InteractionModuleBase
    {
        public SheetsService SheetsService { get; set; }
        
        // Get info on a user, or the user who invoked the command if one is not specified
        [SlashCommand("userinfo", "Shows user info")]
        public async Task UserInfoAsync(IUser user = null)
        {
            string output = "";
            user = user ?? Context.User;
            output += "Username: " + user.Username;
            output += "\nDisplay Name: " + user.GlobalName + "\nServer Nickname: ";
            if (((user as IGuildUser).DisplayName) == null) output += "None";
            else output += (user as IGuildUser).DisplayName;
            output += "\nRoles: ";
            ulong[] idList = (user as IGuildUser).RoleIds.ToArray();
            int size = idList.Count();
            for (int i = 1; i < size; i++)
            {
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == idList[i]);
                if (i == size - 1)
                {
                    output += role.Name;
                }
                else
                {
                    output += role.Name + ", ";
                }
            }
            await RespondAsync(output);
        }

        [SlashCommand("roll", "[amount of dice]d[type of dice]")]
        public async Task RollAsync(string input)
        {
            if (input.ToLower() == "help" || input.ToLower() == "")
            {
                await RespondAsync("$roll [amount of dice]d[type of dice]\nFor Example, $roll 2d6 will roll 2 d6"
                             + " and $roll d6 will roll 1 d6.");
            }
            else
            {
                if (!input.Contains("d"))
                {
                    await RespondAsync("$roll [amount of dice]d[type of dice]\nFor Example, $roll 2d6 will roll 2 d6"
                             + " and $roll d6 will roll 1 d6.");
                }
                else
                {
                    string[] splitInput = input.Split('d');
                    int amount;
                    if (splitInput[0] == "")
                    {
                        amount = 1;
                    }
                    else
                    {
                        amount = Convert.ToInt32(splitInput[0]);
                    }

                    int size = Convert.ToInt32(splitInput[1]);
                    if (amount > 20 || amount < 1 || size <= 1)
                    {
                        await RespondAsync("Amount of dice is incorrect. (20 or less)");
                    }

                    else
                    {
                        Random rnd = new Random();
                        int[] results = new int[20];
                        int total = 0;
                        string output = "";
                        if (amount == 1)
                        {
                            int result = rnd.Next(1, size + 1);
                            output += "You rolled a " + result + ".";
                        }

                        else
                        {
                            for (int i = 0; i < amount; i++)
                            {
                                int result = rnd.Next(1, size + 1);
                                output += result + ", ";
                                total += result;
                                results[i] = result;
                            }
                            output += "Total: " + total;
                        }
                        await RespondAsync(output);
                    }
                }
            }
        }

        [SlashCommand("rng", "Goofy ahhh command")]
        public async Task RngAsync()
        {
            int successes = 0;
            int total = 0;
            var rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                int chance = (rand.Next(99) + 1);
                if (chance <= 5) successes++;
                total++;
            }
            double average = (Convert.ToDouble(successes) / Convert.ToDouble(total)) * 100;
            var hundred = successes;
            string every100 = successes.ToString();
            while (!(4.9 < average && average < 5.1))
            {
                int chance = (rand.Next(99) + 1);
                if (chance <= 5) successes++;
                total++;
                average = (Convert.ToDouble(successes) / Convert.ToDouble(total)) * 100;
                if (total % 100 == 0)
                {
                    every100 += ", " + (successes - hundred);
                    hundred = successes;
                }
            }
            await RespondAsync("It took you " + total + " attempts to reach average RNG.\n" +
                             successes + "/" + total + " (" + average + "%)\nSuccesses for each hundred: " + every100);
        }

        [SlashCommand("elten", "simulate the circles")]
        public async Task EltenAsync()
        {
            int total = 0;
            var rand = new Random();
            bool get = true;
            while (get)
            {
                int chance = rand.Next(58823) + 1;
                if (chance == 1)
                {
                    get = false;
                }
                total++;
            }
            await RespondAsync(total.ToString("D") + " Eltens were killed in the making of this simulation.");
        }
    }
}
