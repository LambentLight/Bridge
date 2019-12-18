using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;

namespace LambentLight.Bridge
{
    /// <summary>
    /// The main class for the bridge.
    /// </summary>
    public class Bridge : BaseScript
    {
        public Bridge()
        {
            // Add the commands that we need
            API.RegisterCommand("bridgekickall", new Action<int, List<object>, string>((s, a, r) => CommandKickAll(s, a, r)), false);
        }

        private void CommandKickAll(int source, List<object> args, string raw)
        {
            // If there is more than one argument, select the first one and use it as the kick message
            // Otherwise, just say that the server is closing down
            string message = args.Count >= 1 ? args[0].ToString() : "Server is shutting down.";

            // Write a console message
            Console.WriteLine($"Kicking all players with message '{message}'");

            // Iterate over the players on the server
            foreach (Player player in Players)
            {
                // And kick every single one of them
                player.Drop(message);
            }
        }
    }
}
