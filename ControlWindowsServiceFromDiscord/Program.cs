using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWindowsServiceFromDiscord
{
    internal class Program
    {
        public static Config Config;

        static void Main(string[] args)
        {
            Console.WriteLine("Windows Service Controller From Discord");
            Console.WriteLine();

            if (!File.Exists("config.yaml"))
            {
                Console.WriteLine("No configuration file found!");
                Environment.Exit(1);
            }

            var ser = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            var fs = new StreamReader("config.yaml", Encoding.UTF8);
            Config = ser.Deserialize<Config>(fs);

            new BotApp().MainAsync().GetAwaiter().GetResult();
        }
    }
}
