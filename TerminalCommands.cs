using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dark_Age_of_Valheim;

public static class TerminalCommands
{
    private static bool isServer => SystemInfo.graphicsDeviceType
                                    == GraphicsDeviceType.Null;
    private static string modName => DarkAgeOfValheim.MOD_NAME;
    private static Localization local => DarkAgeOfValheim.localization;

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZrouteMethodsServerFeedback
    {
        private static void Postfix()
        {
            if (isServer) return;
            //Increase player EXP by set amount.
            ZRoutedRpc.instance.Register($"{modName} terminal_AddExperience",
                new Action<long, int>(RPC_AddExperience));
        }
    }
    private static void RPC_AddExperience(long sender, int amount)
    {
        terminalAddExperience(amount);
        //Chat.instance.RPC_ChatMessage(200, Vector3.zero, 0, local["$notify"], String.Format(local["$terminal_add_experience"], amount), PrivilegeManager.GetNetworkUserId());
    }




    public static void terminalAddExperience(int amount)
    {
        EpicMMOSystem.LevelSystem.Instance.AddExp(amount);
    }

    [HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
    public class AddChatCommands
    {
        [HarmonyPriority(Priority.Low)]
        private static void Postfix()
        {
            long? getPlayerId(string name)
            {
                var clearName = name.Replace('&', ' ');
                var players = ZNet.instance.GetPlayerList();
                foreach (var playerInfo in players)
                {
                    if (playerInfo.m_name == clearName)
                    {
                        return playerInfo.m_characterID.m_userID;
                    }
                }
                return null;
            }
            _ = new Terminal.ConsoleCommand("DarkAgeOfValheim", "Manages admin commands for DAoV.",
                (Terminal.ConsoleEvent)(
                    args =>
                    {
                        //if (!DarkAgeOfValheim.ConfigSync.IsAdmin)
                        //{
                        //    if (ZNet.instance.IsServer())
                        //    {

                        //    }
                        //    else
                        //    {
                        //        args.Context.AddString("You are not an admin on this server.");
                        //        return;
                        //    }
                        //}
                        switch (args[1])
                        {
                            case "add_exp":
                                {
                                    int amount = Int32.Parse(args[2]);
                                    string name = args[3];
                                    if (args.Length > 4)
                                    {
                                        for (var i = 4; i < args.Length; i++)
                                        {
                                            name += " " + args[i];
                                        }

                                    }
                                    var userId = getPlayerId(name);
                                    if (userId == null)
                                    {
                                        DarkAgeOfValheim.print("Player is not found");
                                    }
                                    ZRoutedRpc.instance.InvokeRoutedRPC(userId ?? 200, $"{modName} terminal_AddExperience", amount);
                                    break;
                                }
                            default: break;
                        }

                        args.Context.AddString("set_exp [value] [name] - add experience to player character");
                    }),
                optionsFetcher: () => new List<string>
                    {  "add_exp" });
        }
    }
}
