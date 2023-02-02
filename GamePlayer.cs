using Dark_Age_of_Valheim.Abilities;
using Dark_Age_of_Valheim.LevelSystem;
using Dark_Age_of_Valheim.Specalizations;
using EpicMMOSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using static Dark_Age_of_Valheim.GlobalConstants;

namespace Dark_Age_of_Valheim;

public partial class GamePlayer
{


    public const string epicMMOPluginKey = "EpicMMOSystem";
    public const string middleKey = "LevelSystem";
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
    public Specialization? Specalization { get; set; } = null;


    //Points used to invest in the skill tree
    public int? skillPoints { get; set; } = 0;
    //Abilities the player has learned.
    public List<Ability>? Abilities = new List<Ability>();

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
        DarkAgeOfValheim.LLogger.LogInfo($"Loading player {Player.m_localPlayer.GetPlayerName()}");
        this.Init();
    }

    private void Init()
    {
        loadPlayer();
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
        saveSpecialization();
    }
    public void loadPlayer()
    {
        Player.m_localPlayer.m_knownTexts[$"{DarkAgeOfValheim.MOD_GUID}_specialization"] = "1";
        loadSpecialization();
    }

    protected void saveSpecialization()
    {
        if (!Player.m_localPlayer) return;
        if (Specalization == null) return;

        Player.m_localPlayer.m_knownTexts[$"{DarkAgeOfValheim.MOD_GUID}_specialization"] = Specalization.id.ToString();
    }

    protected void loadSpecialization()
    {
        try
        {
            if (Player.m_localPlayer.m_knownTexts.ContainsKey($"{DarkAgeOfValheim.MOD_GUID}_specialization"))
            {
                byte specId = Convert.ToByte(Player.m_localPlayer.m_knownTexts[$"{DarkAgeOfValheim.MOD_GUID}_specialization"]);
                Specalization = DarkAgeOfValheim.Instance.specializations.Find(i => i.id == (byte)specId);
                DarkAgeOfValheim.LLogger.LogInfo(String.Format("Loading player {0} with class {1}",
                    Player.m_localPlayer.GetPlayerName(),
                    Specalization.name));
            }
        } catch(Exception e)
        {
            DarkAgeOfValheim.LLogger.LogError(e);
        }
    }
}
