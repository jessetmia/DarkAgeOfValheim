using Dark_Age_of_Valheim.Abilities;
using System;
using System.Collections.Generic;

namespace Dark_Age_of_Valheim.Specalizations;

public interface ISpecalization
{
    byte id { get; }
    string name { get; }
    string description { get; }

    //Dictionary<int, string> title { get; }

    //Different classes will have more HP depending on spec. 
    int bonusHp { get; }

    //Starting Stamina Bonus
    int bonusStamina { get; }

    //Starting Mana
    int bonusEitr { get; }

    //Custom Skills for the class (dual wield for berserker, stormcalling for thane, etc)
    Skills[]? skillLines { get; }

    Skills[]? acceleratedSkills { get; }

    AbilityHandler[]? classAbilities { get; }

    //string getTitle(GamePlayer player, int level);
}
