using BepInEx;
using Dark_Age_of_Valheim.Abilities;
using Dark_Age_of_Valheim.EpicLoot;
using Dark_Age_of_Valheim.LevelSystem;
using Dark_Age_of_Valheim.Specalizations;
using EpicLoot;
using EpicMMOSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Dark_Age_of_Valheim;

public partial class GamePlayer
{

    /**
     * 
     * @TODO: Split Buffed Stats from Armor and from buffs/pots.
     * Used for calculation of stats. E.G. getParameter(Parameter.Strenghth) + buffedStrength will yield actual stats.
     *
     */

    public int buffedStrength { get; set; } = 0;
    public int buffedAgility { get; set; } = 0;
    public int buffedIntellect { get; set; } = 0;
    public int buffedBody { get; set; } = 0;

    //Player Class (Healer, Valkrye, Berserker, etc)
    public ISpecalization? Specalization { get; set; } = null;

    //Abilities the player has learned.
    public AbilityHandler[]? Abilities { get; set; } = null;

    //Points used to invest in the skill tree
    public int? skillPoints { get; set; } = 0;

    public const string epicMMOPluginKey = "EpicMMOSystem";
    public const string middleKey = "LevelSystem";

    public Dictionary<Parameter, int> statBuffs = new Dictionary<Parameter, int>();


    #region Singlton
    private static GamePlayer? _instance;
    public static GamePlayer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GamePlayer();
                return _instance;
            }
            return _instance;
        }
    }
    #endregion



    public GamePlayer()
    {
        this.Init();
    }

    private void Init()
    {
        LevelSystemPatch.OnLevelUp += LevelUpAnnouncement;
    }

    public int getBonusPointsForParameter(Parameter parameter)
    {
        switch(parameter)
        {
            case Parameter.Strength:
                {
                    return this.buffedStrength;
                }
            case Parameter.Intellect:
                {
                    return this.buffedIntellect;
                }
            case Parameter.Agility:
                {
                    return this.buffedAgility;
                }
            case Parameter.Body:
                {
                    return this.buffedBody;
                }
        }
        return 0;
    }

    /*
     * Notifies clients that Player has leveled up every 10 levels.
     */
    public void LevelUpAnnouncement(int level)
    {
        //Add config to enable level notifications 
        if (level % 10 != 0) return;
        Player localPlayer = Player.m_localPlayer;
        string text = string.Format("Player {0} has reached level {1}!", localPlayer.GetPlayerName(), level);
        Chat.instance.RPC_ChatMessage(200, Vector3.zero, 0, "<color=#424242>Notice</color>", text, PrivilegeManager.GetNetworkUserId());
    }


    //@TODO: Move this to the player save file instead of random text.
    public void savePlayer()
    {

    }
}
