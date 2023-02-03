using BepInEx;
using BepInEx.Logging;
using Dark_Age_of_Valheim.Abilities;
using Dark_Age_of_Valheim.Specalizations;
using EpicLoot.Data;
using fastJSON;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    internal const string DATA_LOCATION = "Data";

    private Harmony _harmony = new(MOD_GUID);


    public static Localization? localization;

    public static readonly ManualLogSource LLogger =
        BepInEx.Logging.Logger.CreateLogSource(MOD_NAME);

    public static List<Specialization> specializations = new List<Specialization>();
    public static List<Ability> abilities = new List<Ability>();


    //Will never actually be null. 
    //private ConfigEntry<bool>? _serverConfigLocked;

    //private static readonly ConfigSync ConfigSync = new(MOD_GUID)
    //{ DisplayName = MOD_NAME, CurrentVersion = MOD_VERSION, MinimumRequiredVersion = MOD_VERSION };

    //public static readonly ConfigSync ConfigSync = new(DarkAgeOfValheimCFG.MOD_GUID)
    //{ DisplayName = DarkAgeOfValheimCFG.MOD_NAME, CurrentVersion = DarkAgeOfValheimCFG.MOD_VERSION, MinimumRequiredVersion = DarkAgeOfValheimCFG.MOD_VERSION };

    internal static DarkAgeOfValheim Instance;

    [UsedImplicitly]
    public void Awake()
    {
        try
        {
            Game.isModded = true;
            Instance = this;

            localization = new Localization();

            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            loadAbilities();
            loadSpecializations();
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

    protected void loadSpecializations()
    {
        string classData = loadJson("specializations.json");
        if (String.IsNullOrEmpty(classData)) 
        {
            return;
        }
        try
        {
            specializations = JsonConvert.DeserializeObject<List<Specialization>>(classData);

            foreach (Specialization spec in specializations)
            {
                Logger.LogInfo(String.Format("Initializing class: {0}.",spec.name));
            }


        } catch (Exception e)
        {
            Logger.LogError(e);
        }
        return;
    }

    protected void loadAbilities()
    {
        string classData = loadJson("abilities.json");
        if (String.IsNullOrEmpty(classData))
        {
            return;
        }
        try
        {
            abilities = JsonConvert.DeserializeObject<List<Ability>>(classData);

            foreach (Ability ability in abilities)
            {
                Logger.LogInfo(String.Format("Initializing Ability: {0}.", ability.name));
            }


        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
        return;

    }

    public string loadJson(string filePath)
    {
        try
        {
            string file = Path.Combine(Paths.PluginPath, "Fistekefs-Dark Age of Valheim", DATA_LOCATION, filePath);
            if (!File.Exists(file))
            {
                LLogger.LogError("Unable to find file: " + file);
                return String.Empty;
            }
            return File.ReadAllText(file);
        } catch (Exception e)
        {
            Logger.LogError(e);
        }
        return String.Empty;
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        Logger.LogInfo($"Plugin {MOD_GUID} Unloaded");
        Instance = null;
    }
}