using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmongServers.Launcher
{
    /// <summary>
    /// Provides functions to alter region data and launch the game.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Replaces the region info file.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <returns></returns>
        public static async Task ReplaceRegionInfoAsync(IPEndPoint serverEndpoint)
        {
            //TODO:
        }

        /// <summary>
        /// Launches the game.
        /// </summary>
        /// <returns></returns>
        public static Task LaunchGameAsync()
        {
            return Task.Run(() => {
                Process.Start(new ProcessStartInfo("steam://rungameid/945360") {
                    UseShellExecute = true
                });
            });
        }
    }
}
