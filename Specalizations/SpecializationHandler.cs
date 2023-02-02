using Dark_Age_of_Valheim.Abilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dark_Age_of_Valheim.Specalizations;


//It's still possible that I will convert classes to class files instead of json objects.
[Serializable]
public class Specialization
{ 

    public byte id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    //public Dictionary<int, string> title => new Dictionary<int, string>();

    public int bonusHp { get; set;  }

    public int bonusStamina { get; set; }

    public int bonusEitr { get; set; }

    public Skills[]? skillLines { get; set; }

    public Skills[]? acceleratedSkills { get; set; }

    public Ability[]? classAbilities { get; set; }

    //public string getTitle(GamePlayer player, int level)
    //{
    //    return "";
    //}

}
    
[HarmonyPatch(typeof(Player))]
public static class SpecializationBonusHandler
{

    //Add bonus stats before any other calculations
    [HarmonyPriority(Priority.First)]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Player.GetTotalFoodValue))]
    public static void AddBonusStats(ref float stamina, ref float hp, ref float eitr)
    {
        DarkAgeOfValheim.LLogger.LogInfo(GamePlayer.Instance.Specalization?.bonusHp);
        hp += GamePlayer.Instance.Specalization?.bonusHp ?? 0;
        stamina += GamePlayer.Instance.Specalization?.bonusStamina ?? 0;
        eitr += GamePlayer.Instance.Specalization?.bonusEitr ?? 0;
    }
}
