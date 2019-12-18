using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LambentLight.Bridge.Server
{
    /// <summary>
    /// The main class for the bridge.
    /// </summary>
    public class BridgeServer : BaseScript
    {
        public BridgeServer()
        {
            // Add the commands that we need
            API.RegisterCommand("bridgekickall", new Action<int, List<object>, string>((s, a, r) => CommandKickAll(s, a, r)), false);
            API.RegisterCommand("bridgenotify", new Action<int, List<object>, string>((s, a, r) => CommandNotify(s, a, r)), false);
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

        private void CommandNotify(int source, List<object> args, string raw)
        {
            // If there is no arguments provided, notify the user and return
            if (args.Count == 0)
            {
                Console.WriteLine("ERROR: You have not provided a text to use for the notification");
                return;
            }

            // Create a StringBuilder for creating the message
            StringBuilder builder = new StringBuilder();
            // Iterate over the arguments
            for (int i = 0; i < args.Count; i++)
            {
                // And add them into the builder
                builder.Append(args[i]);
                // If this is not the last item, add a space
                if (i != args.Count - 1)
                {
                    builder.Append(" ");
                }
            }

            // Trigger the chat message for all of the players
            TriggerClientEvent("chat:addMessage", new
            {
                color = new[] { 113, 219, 212 },
                multiline = false,
                args = new[] { "LambentLight", builder.ToString() }
            });
            // And notify the user on the console
            Console.WriteLine($"The message '{builder.ToString()}' was sent to all connected players");
        }
    }
}
