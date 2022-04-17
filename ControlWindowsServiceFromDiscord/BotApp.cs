using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Discord.WebSocket;
using System.Threading;

namespace ControlWindowsServiceFromDiscord
{
    internal class BotApp
    {
        private readonly DiscordSocketClient Client;

        internal BotApp()
        {
            Client = new DiscordSocketClient();

            Client.Log += Client_Log;
            Client.Ready += Client_Ready;
            Client.MessageReceived += Client_MessageReceived;
        }

        internal async Task MainAsync()
        {
            await Client.LoginAsync(Discord.TokenType.Bot, Program.Config.BotToken);

            await Client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.Id == Client.CurrentUser.Id) return Task.CompletedTask;

            string srvPrefix = "!sc " + Program.Config.ServerName + ' ';

            if (!arg.Content.StartsWith(srvPrefix)) return Task.CompletedTask;

            if (!Program.Config.Users.ContainsKey(arg.Author.Id)) return Task.CompletedTask;

            int permission = Program.Config.Users[arg.Author.Id];

            string cmdstr = arg.Content.Substring(srvPrefix.Length);
            string[] cmd = cmdstr.Split(' ');

            ServiceController sc = null;
            if (cmd.Length > 1)
            {
                if (!Program.Config.ServiceNameMap.ContainsKey(cmd[1]))
                {
                    arg.Channel.SendMessageAsync("The specified service does not exist as an installed service.");
                    return Task.CompletedTask;
                }
                sc = new ServiceController(Program.Config.ServiceNameMap[cmd[1]] ?? cmd[1]);
            }

            Console.WriteLine('!' + cmdstr);

            try
            {
                switch (cmd[0])
                {
                    case "query":
                        if (permission < 1) arg.Channel.SendMessageAsync("Access is denied.");
                        QueryStatusAndSendInfo(sc, arg.Channel);
                        break;

                    case "start":
                        if (permission < 2) arg.Channel.SendMessageAsync("Access is denied.");
                        sc.Start();
                        QueryStatusAndSendInfo(sc, arg.Channel);
                        break;

                    case "stop":
                        if (permission < 2) arg.Channel.SendMessageAsync("Access is denied.");
                        sc.Stop();
                        QueryStatusAndSendInfo(sc, arg.Channel);
                        break;
                    case "restart":
                        if (permission < 2) arg.Channel.SendMessageAsync("Access is denied.");
                        if (sc.Status==ServiceControllerStatus.Running)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.StopPending);
                            QueryStatusAndSendInfo(sc, arg.Channel);
                        }
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.StartPending);
                        QueryStatusAndSendInfo(sc, arg.Channel);
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                arg.Channel.SendMessageAsync(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                arg.Channel.SendMessageAsync("Unknown Error");
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        private void QueryStatusAndSendInfo(ServiceController sc, ISocketMessageChannel channel)
        { 
            string msg = "```\n" + sc.Status.ToString() + "\n```";
            channel.SendMessageAsync(msg);
        }

        private Task Client_Ready()
        {
            Console.WriteLine("READY!");
            return Task.CompletedTask;
        }

        private Task Client_Log(Discord.LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }
    }
}
