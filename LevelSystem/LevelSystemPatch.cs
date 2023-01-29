using BepInEx;
using EpicMMOSystem;
using HarmonyLib;
using System;

namespace Dark_Age_of_Valheim.LevelSystem;

[HarmonyPatch(typeof(EpicMMOSystem.LevelSystem))]
public static class LevelSystemPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(EpicMMOSystem.LevelSystem.getParameter))]
    public static void AddBuffCalculationToParameters(ref int __result, Parameter parameter)
    {
        GamePlayer player = GamePlayer.Instance;
        string paramKey = $"{GamePlayer.epicMMOPluginKey}_{GamePlayer.middleKey}_{parameter}";
        int buffParam = Convert.ToInt32(player.GetType().GetProperty("buffed" + parameter)?.GetValue(player));
        //DarkAgeOfValheim.LLogger.LogInfo(parameter+ ": "+ buffParam);
        __result += buffParam;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(EpicMMOSystem.LevelSystem.getFreePoints))]
    //@TODO: Refactor so that system assigns freepoints on levelup event instead. 
    /*
     * 
     * 
     * 
 [Error  :  HarmonyX] Failed to patch int EpicMMOSystem.LevelSystem::getFreePoints(): 
    HarmonyLib.InvalidHarmonyPatchArgumentException: (static bool Dark_Age_of_Valheim.LevelSystem.LevelSystemPatch::getFreePointsSubBuffedPoints(Int32& __result)): 
    Return type of pass through postfix static bool Dark_Age_of_Valheim.LevelSystem.LevelSystemPatch::getFreePointsSubBuffedPoints(Int32& __result) does not match 
    type of its first parameter
  at HarmonyLib.Public.Patching.HarmonyManipulator.WritePostfixes (HarmonyLib.Internal.Util.ILEmitter+Label returnLabel) [0x0035a] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.HarmonyManipulator.WriteImpl () [0x00234] in <474744d65d8e460fa08cd5fd82b5d65f>:0
     * 
     */
    public static void getFreePointsSubBuffedPoints(ref int __result)
    {
        var levelPoint = EpicMMOSystem.EpicMMOSystem.freePointForLevel.Value;
        var freePoint = EpicMMOSystem.EpicMMOSystem.startFreePoint.Value;
        EpicMMOSystem.LevelSystem playerInstance = EpicMMOSystem.LevelSystem.Instance;
        var level = playerInstance.getLevel();
        var total = level * levelPoint + freePoint;
        int usedUp = 0;
        for (int i = 0; i < 4; i++)
        {
            usedUp += playerInstance.getParameter((Parameter)i) - GamePlayer.Instance.getBonusPointsForParameter((Parameter)i);
        }

        try
        {
            int addPoints = 0;
            string str = EpicMMOSystem.EpicMMOSystem.levelsForBinusFreePoint.Value;
            if (str.IsNullOrWhiteSpace())
            {
                __result = total - usedUp;
                return;
            }
            var map = str.Split(',');
            foreach (var value in map)
            {
                var keyValue = value.Split(':');
                if (Int32.Parse(keyValue[0]) <= level)
                {
                    addPoints += Int32.Parse(keyValue[1]);
                }
                else
                {
                    break;
                }
            }

            total += addPoints;
        }
        catch (Exception e)
        {
            DarkAgeOfValheim.LLogger.LogError($"Free point, bonus error: {e.Message}");
        }
        __result = total - usedUp;
        return;
    }
}



/*
 [Error  :Dark Age of Valheim] HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.InvalidHarmonyPatchArgumentException: (static bool Dark_Age_of_Valheim.LevelSystem.LevelSystemPatch::getFreePointsSubBuffedPoints(Int32& __result)): Return type of pass through postfix static bool Dark_Age_of_Valheim.LevelSystem.LevelSystemPatch::getFreePointsSubBuffedPoints(Int32& __result) does not match type of its first parameter
  at HarmonyLib.Public.Patching.HarmonyManipulator.WritePostfixes (HarmonyLib.Internal.Util.ILEmitter+Label returnLabel) [0x0035a] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.HarmonyManipulator.WriteImpl () [0x00234] in <474744d65d8e460fa08cd5fd82b5d65f>:0
   --- End of inner exception stack trace ---
  at HarmonyLib.Public.Patching.HarmonyManipulator.WriteImpl () [0x00378] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.HarmonyManipulator.Process (MonoMod.Cil.ILContext ilContext, System.Reflection.MethodBase originalMethod) [0x00042] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.HarmonyManipulator.Manipulate (System.Reflection.MethodBase original, HarmonyLib.PatchInfo patchInfo, MonoMod.Cil.ILContext ctx) [0x00006] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.HarmonyManipulator.Manipulate (System.Reflection.MethodBase original, MonoMod.Cil.ILContext ctx) [0x00007] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Public.Patching.ManagedMethodPatcher.Manipulator (MonoMod.Cil.ILContext ctx) [0x00012] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at MonoMod.Cil.ILContext.Invoke (MonoMod.Cil.ILContext+Manipulator manip) [0x00087] in <6733e342b5b549bba815373898724469>:0
  at MonoMod.RuntimeDetour.ILHook+Context.InvokeManipulator (Mono.Cecil.MethodDefinition def, MonoMod.Cil.ILContext+Manipulator cb) [0x00012] in <4e2760c7517c4ea79c633d67e84b319f>:0
  at (wrapper dynamic-method) MonoMod.RuntimeDetour.ILHook+Context.DMD<MonoMod.RuntimeDetour.ILHook+Context::Refresh>(MonoMod.RuntimeDetour.ILHook/Context)
  at (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<MonoMod.RuntimeDetour.ILHook+Context::Refresh>?879261696(object)
  at HarmonyLib.Internal.RuntimeFixes.StackTraceFixes.OnILChainRefresh (System.Object self) [0x00000] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at MonoMod.RuntimeDetour.ILHook.Apply () [0x00059] in <4e2760c7517c4ea79c633d67e84b319f>:0
  at HarmonyLib.Public.Patching.ManagedMethodPatcher.DetourTo (System.Reflection.MethodBase replacement) [0x00047] in <474744d65d8e460fa08cd5fd82b5d65f>:0
   --- End of inner exception stack trace ---
  at HarmonyLib.Public.Patching.ManagedMethodPatcher.DetourTo (System.Reflection.MethodBase replacement) [0x0005f] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.PatchFunctions.UpdateWrapper (System.Reflection.MethodBase original, HarmonyLib.PatchInfo patchInfo) [0x00033] in <474744d65d8e460fa08cd5fd82b5d65f>:0
   --- End of inner exception stack trace ---
  at HarmonyLib.PatchClassProcessor.ReportException (System.Exception exception, System.Reflection.MethodBase original) [0x00045] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.PatchClassProcessor.Patch () [0x00095] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Harmony.<PatchAll>b__11_0 (System.Type type) [0x00007] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.CollectionExtensions.Do[T] (System.Collections.Generic.IEnumerable`1[T] sequence, System.Action`1[T] action) [0x00014] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at HarmonyLib.Harmony.PatchAll (System.Reflection.Assembly assembly) [0x00006] in <474744d65d8e460fa08cd5fd82b5d65f>:0
  at Dark_Age_of_Valheim.DarkAgeOfValheim.Awake () [0x00023] in <fda757c04a4d4d8aa6b82f0f37a49518>:0
*/