using BepInEx;
using Dark_Age_of_Valheim.Abilities;
using HarmonyLib;
using Newtonsoft.Json;
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

    [JsonConstructor]
    public Specialization(byte id, string name, string description, int bonusHp, int bonusStamina, int bonusEitr, string skillLines, string acceleratedSkillLines, string abilityLines)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.bonusHp = bonusHp;
        this.bonusStamina = bonusStamina;
        this.bonusEitr = bonusEitr;
        this.skillLines = skillLines;
        this.acceleratedSkillLines = acceleratedSkillLines;
        this.abilityLines = abilityLines;
        loadClassAbilities();
    }

    public byte id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    //public Dictionary<int, string> title => new Dictionary<int, string>();

    public int bonusHp { get; set;  }

    public int bonusStamina { get; set; }

    public int bonusEitr { get; set; }
    
    public string? skillLines { get; set; }

    public string? acceleratedSkillLines { get; set; }

    public string? abilityLines { get; set; }

    public List<Skills> classSkills = new List<Skills>();

    public List<Skills> acceleratedSkills = new List<Skills>();

    public List<Ability> classAbilities = new List<Ability>();

    //public string getTitle(GamePlayer player, int level)
    //{
    //    return "";
    //}

    protected void loadClassAbilities()
    {

        //System did not load any abilities
        if (DarkAgeOfValheim.abilities is null) { return; }

        //clear all skills as precation.
        classAbilities.Clear();
        if (abilityLines.IsNullOrWhiteSpace()) { return; }

        string[] abilityGroups = abilityLines.Split(',');
        foreach (string abilityGroup in abilityGroups)
        {
            List<Ability> abilities = DarkAgeOfValheim.abilities.Where(i => i.abilityGroup == abilityGroup).ToList<Ability>();
            if (abilities is null) { return; }

            DarkAgeOfValheim.LLogger.LogInfo(String.Format("Loading Ability Group {0} for Class {1}", abilityGroup, name));
            classAbilities.AddRange(abilities);
        }
    }

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
