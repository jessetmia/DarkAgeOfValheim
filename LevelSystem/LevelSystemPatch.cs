using EpicMMOSystem;
using HarmonyLib;
using System;

namespace Dark_Age_of_Valheim.LevelSystem;

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
     * points for the MMO system. 
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