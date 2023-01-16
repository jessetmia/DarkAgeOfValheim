using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dark_Age_of_Valheim.LevelSystem;

public static class MobDeathEvents
{

    //Rewrite this to handle mob RPC ID for sending package data
    private static readonly Dictionary<Character, long> MobRPCIdDictionary = new();

    // This is used to keep track of players who have attacked an npc. Any reason to make this public?
    // @TODO: How to bind to the mobBrain instead of a dictionary list?
    private static readonly Dictionary<uint, List<Character>> MobAggroDictionary = new();


    //Quick and dirty way to make sure we're looking at a mob being attacked by a player.
    //Rename to something that makes more sense.
    private static bool IsInstanceMobAndAlive(Character character, Character attacker)
    {
        if (character.IsPlayer() || character.IsDead()) return false;
        if (!attacker || !attacker.IsPlayer() || attacker.IsDead()) return false;

        return true;
    }

    // Register RPCs Mob combat RPCs
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class RegisterRpc
    {

        public static void Postfix()
        {
            ZRoutedRpc.instance.Register($"{DarkAgeOfValheim.MOD_NAME} AddPlayersToMobAggroList", new Action<long, ZPackage>(RPC_AddPlayersToMobAggroList));
            ZRoutedRpc.instance.Register($"{DarkAgeOfValheim.MOD_NAME} OnMobDeath", new Action<long, ZPackage>(RPC_OnMobDeath));
        }
    }

    public static void RPC_AddPlayersToMobAggroList(long send, ZPackage pkg)
    {
        try
        {
            if (!Player.m_localPlayer) return;
            if (Player.m_localPlayer.IsDead()) return;

            long playerId = pkg.ReadLong();
            DarkAgeOfValheim.LLogger.LogInfo("Player Id: " + playerId);
            uint mobId = pkg.ReadUInt();
            DarkAgeOfValheim.LLogger.LogInfo("Mob Id: " + mobId);

            if (!MobAggroDictionary.ContainsKey(mobId))
            {

                DarkAgeOfValheim.LLogger.LogInfo("Creating Dictionary Entry for " + mobId);
                MobAggroDictionary.Add(mobId, new List<Character>());
            }

            foreach (var player in Player.m_players)
            {
                DarkAgeOfValheim.LLogger.LogInfo("PlayerId: " + player.GetPlayerID());
                if (player.GetPlayerID() != playerId) continue;
                if (MobAggroDictionary[mobId].Contains(player)) break;

                //Add player to Mobs aggro table.
                DarkAgeOfValheim.LLogger.LogInfo(String.Format("Adding Player {0} to Mob {1}'s aggro table.", player.GetPlayerName(), mobId));
                MobAggroDictionary[mobId].Add(player);
            }
        }
        catch (Exception e)
        {
            DarkAgeOfValheim.LLogger.LogError(e);
        }
        return;
    }

    /**
     * 
     * Handle logic after mob killed. 
     * 
     */
    public static void RPC_OnMobDeath(long sender, ZPackage pkg)
    {
        uint mobId = pkg.ReadUInt();
        int mobMaxHealth = pkg.ReadInt();
        Vector3 position = pkg.ReadVector3();
        int xpMultiplyer = pkg.ReadBool() ? 3 : 1;

        string debug = String.Format("mobId {0}; mobMaxHealth {1} position {2} xpMultiplyer {3}", mobId, mobMaxHealth, position, xpMultiplyer);

        DarkAgeOfValheim.LLogger.LogInfo(debug);

        /**
         * @TODO: Make logger broadcast from server
         */
        //if ((double)Vector3.Distance(position, Player.m_localPlayer.transform.position) >= 50f)
        //{
        DarkAgeOfValheim.LLogger.LogInfo("Mob Position: " + position);
        DarkAgeOfValheim.LLogger.LogInfo("Player is not close. No Xp Awarded. Position: " + Player.m_localPlayer.transform.position);
        DarkAgeOfValheim.LLogger.LogInfo("Vector3.Distance: " + Vector3.Distance(position, Player.m_localPlayer.transform.position));
        //return;
        //}

        int baseXp = mobMaxHealth;


        var playerExp = baseXp * xpMultiplyer;
        DarkAgeOfValheim.LLogger.LogInfo("Player Exp Is " + playerExp);

        if (MobAggroDictionary[mobId].Contains(Player.m_localPlayer))
        {
            DarkAgeOfValheim.LLogger.LogInfo(String.Format("Found Player {0} in MobAggroDictionary", Player.m_localPlayer.GetPlayerName()));
            EpicMMOSystem.LevelSystem.Instance.AddExp(playerExp);
        }

        return;
    }

    /**
  * Only for NPCs for now. Will need to think of how to properly manage players for pvp experience
  * If we add taunts, etc down the line, we may have to refactor to add threat, etc.
  * For now this is only used to handle xp generation after kill. 
  */
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    static class AddPlayersToMobAggroList
    {
        public static void Prefix(Character __instance, long sender, HitData hit)
        {
            try
            {
                Character attacker = hit.GetAttacker();
                //Will need to think about how to handle PVP and xp in a way that isn't exploitable.
                if (!IsInstanceMobAndAlive(__instance, attacker)) return;

                Player player = attacker as Player;

                var pkg = new ZPackage();
                pkg.Write(player.GetPlayerID());
                pkg.Write(__instance.GetZDOID().m_id);
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, $"{DarkAgeOfValheim.MOD_NAME} AddPlayersToMobAggroList", new object[] { pkg });

            }
            catch (Exception e)
            {
                DarkAgeOfValheim.LLogger.LogError(e);
            }

            return;
        }
    }


    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    static class GrantXpAndClearAggroList
    {
        public static void Postfix(Character __instance, HitData hit)
        {

            try
            {
                Character attacker = hit.GetAttacker();

                //Will revist for PvP purposes.
                if (!IsInstanceMobAndAlive(__instance, attacker)) return;
                if (__instance.GetHealth() > 0) return;

                Player player = attacker as Player;

                uint mobId = __instance.GetZDOID().id;

                //No players add to Dictionary Table. No reason to give xp. 
                if (MobAggroDictionary[mobId].Count == 0) return;


                var BossDropFlag = __instance.GetFaction() == Character.Faction.Boss ? true : false;

                var pkg = new ZPackage();
                pkg.Write(mobId); //uint
                pkg.Write(Convert.ToInt32(__instance.GetMaxHealth())); //uint
                pkg.Write(__instance.transform.position); //Vector3
                pkg.Write(BossDropFlag); //Bool

                ZRoutedRpc.instance.InvokeRoutedRPC(attacker.GetZDOID().userID, $"{DarkAgeOfValheim.MOD_NAME} DeadMonsters", new object[] { pkg });
            }
            catch (Exception e)
            {

            }
        }
    }


    [HarmonyPatch(typeof(Character), nameof(Character.OnDestroy))]
    static class Character_OnDestroy_Patch
    {
        static void Postfix(Character __instance)
        {
            uint mobId = __instance.GetZDOID().id;
            if (MobRPCIdDictionary.ContainsKey(__instance))
            {
                DarkAgeOfValheim.LLogger.LogInfo("Removing Mob From RPC Dictionary");
                MobRPCIdDictionary.Remove(__instance);
            }

            if (MobAggroDictionary.ContainsKey(mobId))
            {
                DarkAgeOfValheim.LLogger.LogInfo("Removing Mob and players From Aggro Dictionary");
                MobAggroDictionary.Remove(mobId);
            }

        }
    }

}
