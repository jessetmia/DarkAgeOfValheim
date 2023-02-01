using Dark_Age_of_Valheim.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dark_Age_of_Valheim.Specalizations;


//It's still possible that I will convert classes to class files instead of json objects.
[Serializable]
public class Specialization : ISpecalization
{

    public byte id { get; } = 0;
    public string name { get; } = "Unknown Class";
    public string description { get; } = "";
    //public Dictionary<int, string> title => new Dictionary<int, string>();

    public int bonusHp { get; } = 0;

    public int bonusStamina { get; } = 0;

    public int bonusEitr { get; } = 0;

    public Skills[]? skillLines { get; } = null;

    public Skills[]? acceleratedSkills { get; } = null;

    public AbilityHandler[]? classAbilities { get; } = null;

    //public string getTitle(GamePlayer player, int level)
    //{
    //    return "";
    //}

}
