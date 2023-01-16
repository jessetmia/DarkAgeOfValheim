namespace Dark_Age_of_Valheim;

public class Localization
{
    public Localization()
    {
        var currentLanguage = global::Localization.instance.GetSelectedLanguage();
        DarkAgeOfValheim.LLogger.LogInfo(currentLanguage.ToString());
    }

}
