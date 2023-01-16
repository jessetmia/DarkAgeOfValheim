using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace Dark_Age_of_Valheim;

public static class DarkAgeOfValheimCFG
{
}


[BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
public class DarkAgeOfValheim : BaseUnityPlugin
{

    // Mod CFG descriptors. 
    internal const string MOD_GUID = "fistekefs.Dark_Age_of_Valheim";
    internal const string MOD_NAME = "Dark Age of Valheim";
    internal const string MOD_VERSION = "1.0.0";
    internal const string MOD_DESCRIPTION = "Dark Age of Valheim TC for Valheim";

    private Harmony? _harmony;

    private ConfigEntry<string> configGreeting;
    private ConfigEntry<bool> configDisplayGreeting;


    public static Localization? localization;

    public static readonly ManualLogSource LLogger =
        BepInEx.Logging.Logger.CreateLogSource(MOD_NAME);

    //public static readonly ConfigSync ConfigSync = new(DarkAgeOfValheimCFG.MOD_GUID)
    //{ DisplayName = DarkAgeOfValheimCFG.MOD_NAME, CurrentVersion = DarkAgeOfValheimCFG.MOD_VERSION, MinimumRequiredVersion = DarkAgeOfValheimCFG.MOD_VERSION };

    internal static DarkAgeOfValheim? Instance;

    [UsedImplicitly]
    private void Awake()
    {
        try
        {
            Instance = this;
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_GUID);
            Logger.LogInfo($"Plugin {MOD_GUID} is loaded!");
            configGreeting = Config.Bind("General",      // The section under which the option is shown
                                            "GreetingText",  // The key of the configuration option in the configuration file
                                            "Hello, world!", // The default value
                                            "A greeting text to show when the game is launched"); // Description of the option to show in the config file

            configDisplayGreeting = Config.Bind("General.Toggles",
                                                "DisplayGreeting",
                                                true,
                                                "Whether or not to show the greeting text");

            localization = new Localization();
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }




    [UsedImplicitly]
    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        Instance = null;
    }
}