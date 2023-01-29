using EpicMMOSystem;
using HarmonyLib;
using System;

namespace Dark_Age_of_Valheim;


/**
 * 
 * Quick class to turn skilling up in game to xp for MMO system. 
 * @TODO: Add config option for modifier. Currently hardcoded to 10x xp.
 * 
 **/
[HarmonyPatch(typeof(Skills.Skill), nameof(Skills.Skill.Raise))]
public static class ExpGainBySkillRaise
{
    private static void Postfix(Skills.Skill __instance, float factor)
    {
        try
        {
            int playerExp = calculateExp(factor);
            EpicMMOSystem.LevelSystem.Instance.AddExp(playerExp);

        }
        catch (NullReferenceException exception)
        {
            DarkAgeOfValheim.LLogger.LogError(exception);
        }
    }

    private static int calculateExp(float exp)
    {
        return Convert.ToInt32(exp * 10);
    }
}