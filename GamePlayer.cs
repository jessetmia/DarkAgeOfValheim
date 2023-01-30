using BepInEx;
using Dark_Age_of_Valheim.EpicLoot;
using Dark_Age_of_Valheim.LevelSystem;
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
    protected int _buffedStrength = 0;
    protected int _buffedAgility = 0;
    protected int _buffedIntellect = 0;
    protected int _buffedBody = 0;

    protected string? _specalization;

    public int buffedStrength
    {
        get { return _buffedStrength; }
        set { _buffedStrength = value; }
    }
    public int buffedAgility
    {
        get { return _buffedAgility; }
        set { _buffedAgility = value; }
    }
    public int buffedIntellect
    {
        get { return _buffedIntellect; }
        set { _buffedIntellect = value; }
    }
    public int buffedBody
    {
        get { return _buffedBody; }
        set { _buffedBody = value; }
    }

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

    public void addBonusPointToParameter(Parameter parameter, int bonusPoints)
    {
        if (!Player.m_localPlayer) return;
        //In future should we look at not allowing buff cap to go past max? 
        //int max = EpicMMOSystem.maxValueAttribute.Value;
        //int setValue = Mathf.Clamp(value, 0, max);
        //Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_{parameter.ToString()}_bonus"] = bonusPoints.ToString();

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
     * 
     * Maybe every 10 levels? (level%10 == 0)
     * Should be able to disable as well.
     * Notifies clients that Player has leveled up.
     * 
     */
    public void LevelUpAnnouncement(int level)
    {
        DarkAgeOfValheim.LLogger.LogInfo("Calling LevelUpAnnouncement()");
        Player localPlayer = Player.m_localPlayer;
        string text = string.Format("Player {0} has reached level {1}",localPlayer.GetPlayerName(), level);
        Chat.instance.SendText(Talker.Type.Normal, text);
    }
}

/**
  * 
  * Strength = 0, Agility = 1, Intellect = 2, Body = 3
  * Instead of adding a straight value, we should look at incorporating as a buff bonus.e.g.Base Parameter Strength = 200, Buff Bonus + 20. 
  * This will also work with buff pots to stack on top of the base. 
  * This will probably require me to rewrite EpicMMO to handle properly.
  * Instead of saving as a knownText, we should probably just save a variable as it changes often. 
  * 
  **/
