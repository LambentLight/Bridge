﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LambentLight.Bridge.Server
{
    /// <summary>
    /// The main class for the bridge.
    /// </summary>
    public class BridgeServer : BaseScript
    {
        /// <summary>
        /// The address of the LambentLight API.
        /// </summary>
        private static readonly string Bind = API.GetConvar("lambentlight_bind", "");
        /// <summary>
        /// The Auth token for the LambentLight API.
        /// </summary>
        private static readonly string Token = API.GetConvar("lambentlight_token", "");
        /// <summary>
        /// If the LambentLight API is available.
        /// </summary>
        private static bool IsApiAvailable => !(string.IsNullOrWhiteSpace(Bind) || string.IsNullOrWhiteSpace(Token));

        public BridgeServer()
        {
            // If API is not available
            if (!IsApiAvailable)
            {
                Debug.WriteLine("LambentLight Token or Bind Address not found, Bridge running on Manual Mode");
            }
            // Otherwise
            else
            {
                Debug.WriteLine($"LambentLight Token and Bind Address was found ({Bind}), Bridge is fully working");
                $"{Bind}/bridge/available".WithHeader("Authorization", $"Bearer {Token}").PostStringAsync("");
            }

            // Add the commands that we need
            API.RegisterCommand("bridgekickall", new Action<int, List<object>, string>((s, a, r) => CommandKickAll(s, a, r)), false);
            API.RegisterCommand("bridgenotify", new Action<int, List<object>, string>((s, a, r) => CommandNotify(s, a, r)), false);
            API.RegisterCommand("bridgeshutdown", new Action<int, List<object>, string>((s, a, r) => CommandShutdown(s, a, r)), false);
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

        private async void CommandShutdown(int source, List<object> args, string raw)
        {
            // If the API is not available, notify it and return
            if (!IsApiAvailable)
            {
                Debug.WriteLine("This feature cannot be used because the API is not available");
                return;
            }

            // Notify that a shutdown was initiated
            Debug.WriteLine("Shutdown initiated, Bridge will close the server once is completed");

            // Wait until the server is empty
            while (Players.Count() != 0)
            {
                await Delay(0);
            }

            // If the server is empty, make the API call
            await $"{Bind}/bridge/serverempty".WithHeader("Authorization", $"Bearer {Token}").PostStringAsync("");
        }
    }
}
