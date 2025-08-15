using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRage;

namespace IndustrialSystems.Utilities
{
    public class ChatCommands
    {
        public static ChatCommands Instance;

        public const string ModName = "Industrial Systems";

        private MyTuple<ulong, string[]> _lastCommandSent;
        public static MyTuple<ulong, string[]> LastCommandSent => Instance._lastCommandSent;

        private Dictionary<string, MyTuple<Action<ulong, string[]>, string>> _chatCommands;

        public static void OnChatMessageRecieved(ulong sender, string messageText, ref bool sendToOthers)
        {
            string[] messageTextSplit = messageText.Split(' ');

            MyTuple<Action<ulong, string[]>, string> Command;

            if (Instance._chatCommands.TryGetValue(messageTextSplit[0].ToLowerInvariant(), out Command))
            {
                messageTextSplit[0] = null;

                sendToOthers = false;
                string[] copy = new string[messageTextSplit.Length];
                messageTextSplit.CopyTo(copy, 0);
                Instance._lastCommandSent = new MyTuple<ulong, string[]>(sender, copy);
                
                Command.Item1.Invoke(sender, messageTextSplit);
            }
        }

        

        public static void AddChatCommand(string CommandText, Action<ulong, string[]> Command, string description = null)
        {
            if (!Instance._chatCommands.ContainsKey(CommandText.ToLowerInvariant()))
                Instance._chatCommands.Add(CommandText.ToLowerInvariant(), new MyTuple<Action<ulong, string[]>, string>(Command, description));
            else
            {
                MyLog.Default.Warning("Chat command already exists.");
            }
        }
        private static void ChatCommand_GetAllCommands(ulong SenderId, string[] message)
        {
            foreach (KeyValuePair<string, MyTuple<Action<ulong, string[]>, string>> Command in Instance._chatCommands)
            {
                if (!(Command.Key == "/showallcommands"))
                {
                    ShowMessage(Command.Key.ToString());
                    if (Command.Value.Item2 != null)
                        MyAPIGateway.Utilities.ShowMessage("", Command.Value.Item2);
                }
            }

            return;
        }
        private ChatCommands()
        {
            _chatCommands = new Dictionary<string, MyTuple<Action<ulong, string[]>, string>>();
            _lastCommandSent = new MyTuple<ulong, string[]>();
        }

        public static void Load()
        {
            Instance = new ChatCommands();

            MyAPIUtilities.Static.MessageEnteredSender += OnChatMessageRecieved;
            AddChatCommand("/ShowAllCommands", ChatCommand_GetAllCommands);
        }

        public static void Unload()
        {
            MyAPIUtilities.Static.MessageEnteredSender -= OnChatMessageRecieved;
            Instance._chatCommands.Clear();
            Instance = null;
        }

        public static void ShowMessage(string message)
        {
            MyAPIGateway.Utilities.ShowMessage(ModName, message);
        }


        public static bool IsOwner(ulong PlayerId)
        {
            return MyAPIGateway.Session.GetUserPromoteLevel(PlayerId) >= MyPromoteLevel.Owner;
        }
        public static bool IsAdmin(ulong PlayerId)
        {
            return MyAPIGateway.Session.GetUserPromoteLevel(PlayerId) >= MyPromoteLevel.Admin;
        }

        public static bool IsSpaceMaster(ulong PlayerId)
        {
            return MyAPIGateway.Session.GetUserPromoteLevel(PlayerId) >= MyPromoteLevel.SpaceMaster;
        }

        public static bool IsModerator(ulong PlayerId)
        {
            return MyAPIGateway.Session.GetUserPromoteLevel(PlayerId) >= MyPromoteLevel.Moderator;
        }

        public static bool IsOnlyPlayer(ulong PlayerId)
        {
            return MyAPIGateway.Session.GetUserPromoteLevel(PlayerId) == MyPromoteLevel.None;
        }
    }
}
