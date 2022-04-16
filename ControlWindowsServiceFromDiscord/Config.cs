using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWindowsServiceFromDiscord
{
    internal class Config
    {
        public string BotToken { get; set; } = null;

        public string ServerName { get; set; }

        /// <summary>
        /// Key ulong: Discord User ID
        /// Value int: Permission Number
        ///  - 0: Disable Access
        ///  - 1: Can check service status
        ///  - 2: Can Start/Stop service
        /// </summary>
        public Dictionary<ulong, int> Users { get; set; } = new Dictionary<ulong, int>();

        public Dictionary<string, string> ServiceNameMap { get; set; } = new Dictionary<string, string>();
    }
}
