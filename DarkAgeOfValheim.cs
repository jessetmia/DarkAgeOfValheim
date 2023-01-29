using BepInEx;
using BepInEx.Logging;
using EpicLoot.Data;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dark_Age_of_Valheim;

[BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
[BepInDependency("WackyMole.EpicMMOSystem")]
[BepInDependency("randyknapp.mods.epicloot")]
[BepInDependency("org.bepinex.plugins.dualwield")]
public class DarkAgeOfValheim : BaseUnityPlugin
{

    // Mod CFG descriptors. 
    internal const string MOD_AUTHOR = "fistekefs";
    internal const string MOD_GUID = MOD_AUTHOR + ".mods.Dark_Age_of_Valheim";
    internal const string MOD_NAME = "Dark Age of Valheim";
    internal const string MOD_VERSION = "1.0.0";
    internal const string MOD_DESCRIPTION = "Dark Age of Valheim TC for Valheim";

    private Harmony _harmony = new(MOD_GUID);


    public static Localization? localization;

    public static readonly ManualLogSource LLogger =
        BepInEx.Logging.Logger.CreateLogSource(MOD_NAME);


    //Will never actually be null. 
    //private ConfigEntry<bool>? _serverConfigLocked;

    //private static readonly ConfigSync ConfigSync = new(MOD_GUID)
    //{ DisplayName = MOD_NAME, CurrentVersion = MOD_VERSION, MinimumRequiredVersion = MOD_VERSION };

    //public static readonly ConfigSync ConfigSync = new(DarkAgeOfValheimCFG.MOD_GUID)
    //{ DisplayName = DarkAgeOfValheimCFG.MOD_NAME, CurrentVersion = DarkAgeOfValheimCFG.MOD_VERSION, MinimumRequiredVersion = DarkAgeOfValheimCFG.MOD_VERSION };

    internal static DarkAgeOfValheim? Instance;

    [UsedImplicitly]
    public void Awake()
    {
        try
        {
            Game.isModded = true;
            Instance = this;

            localization = new Localization();

            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            var playerLevel = GamePlayer.Instance;
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }


    //public static void InitializeConfig()
    //{
    //    LoadJsonFile<IDictionary<string, object>>("translations.json", LoadTranslations, ConfigType.Nonsynced);
    //}

    //private static void LoadTranslations(IDictionary<string, object> translations)
    //{
    //    const string translationPrefix = "mod_epicloot_";

    //    if (translations == null)
    //    {
    //        DarkAgeOfValheim.LLogger.LogError("Could not parse translations.json!");
    //        return;
    //    }

    //    var oldEntries = Localization.instance.m_translations.Where(instanceMTranslation => instanceMTranslation.Key.StartsWith(translationPrefix)).ToList();

    //    //Clean Translations
    //    foreach (var entry in oldEntries)
    //    {
    //        Localization.instance.m_translations.Remove(entry.Key);
    //    }

    //    //Load New Translations
    //    foreach (var translation in translations)
    //    {
    //        Localization.instance.AddWord(translation.Key, translation.Value.ToString());
    //    }
    //}


    [UsedImplicitly]
    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        Logger.LogInfo($"Plugin {MOD_GUID} Unloaded");
        Instance = null;
    }
}