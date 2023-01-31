using EpicMMOSystem;
using HarmonyLib;
using System;

namespace Dark_Age_of_Valheim.LevelSystem;




/*
 * 
 * @TODO: Rewrite level system to be event based. Current implementation is frustrating to work with. Stats should be loaded onto the character, not loaded every second
 * the game is running. The only time recalc should be called is if the points change which could be handled by events.
 * Instead of saving level data as a knownText, we should look into saving it as a part of the player save file. Valheim Plus had solid example of how to do this. 
 * 
 */

[HarmonyPatch(typeof(EpicMMOSystem.LevelSystem))]
public static class LevelSystemPatch
{

    public static event Action<int>? OnLevelUp;


    [HarmonyPostfix]
    [HarmonyPatch(nameof(EpicMMOSystem.LevelSystem.getParameter))]
    public static void AddBuffCalculationToParameters(ref int __result, Parameter parameter)
    {
        GamePlayer player = GamePlayer.Instance;
        string paramKey = $"{GamePlayer.epicMMOPluginKey}_{GamePlayer.middleKey}_{parameter}";
        int buffParam = Convert.ToInt32(player.GetType().GetProperty("buffed" + parameter)?.GetValue(player));
        __result += buffParam;
    }

    /*
     * 
     * @TODO: Refactor so that system assigns freepoints on levelup event instead. 
     * The system calculates how many points a player should have based on their level. If they have extra points due to buffs
     * This was initially included in the calculation. This system removes the bonus points so that system only calculates the base
     * points for the MMO system. I thought to make this system closer to DAoCs where you have primary, secondary and tertiary stats that
     * increase on level, but I prefer the thought of giving players the ability to customize their builds to the fullest. 
     * 
     */
    [HarmonyPostfix]
    [HarmonyPatch(nameof(EpicMMOSystem.LevelSystem.getFreePoints))]
    public static void getFreePointsSubBuffedPoints(ref int __result)
    {
        try
        {
            EpicMMOSystem.LevelSystem playerInstance = EpicMMOSystem.LevelSystem.Instance;
            int buffPoints = 0;
            for (int i = 0; i < 4; i++)
            {
                buffPoints += -GamePlayer.Instance.getBonusPointsForParameter((Parameter)i);
            }
            __result -= buffPoints;
        }
        catch (Exception e)
        {
            DarkAgeOfValheim.LLogger.LogError($"Free point, bonus error: {e.Message}");
        }
        return;
    }



    [HarmonyPostfix]
    [HarmonyPatch(nameof(EpicMMOSystem.LevelSystem.AddLevel))]
    public static void FireOffLevelEvent()
    {
        OnLevelUp?.Invoke(EpicMMOSystem.LevelSystem.Instance.getLevel());
    }


}