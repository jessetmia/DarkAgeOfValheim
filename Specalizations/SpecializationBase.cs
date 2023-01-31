using Dark_Age_of_Valheim.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dark_Age_of_Valheim.Specalizations;

class SpecializationBase: ISpecalization
{

    public byte id { get; } = 0;
    public string name { get; } = "Unknown Class";

    public string description { get; } = "";

    public int bonusHp { get; } = 0;

    public int bonusStamina { get; } = 0;

    public int bonusEitr { get; } = 0;

    public Skills[]? classSkills { get; } = null;

    public IAbilities[]? classAbilities { get; } = null;

    public string getTitle(GamePlayer player, int level)
    {
        return "";
    }

}
