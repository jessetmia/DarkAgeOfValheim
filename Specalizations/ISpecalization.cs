using Dark_Age_of_Valheim.Abilities;
using System;


namespace Dark_Age_of_Valheim.Specalizations;

public interface ISpecalization
{
    byte id { get; }
    string name { get; }
    string description { get; }

    //Different classes will have more HP depending on spec. 
    int bonusHp { get; }

    //Starting Stamina Bonus
    int bonusStamina { get; }

    //Starting Mana
    int bonusEitr { get; }

    //Skills that the class is best suited for. Bonus xp while leveling skills (Config option for %)
    Skills[]? classSkills { get; }

    IAbilities[]? classAbilities { get; }


    string getTitle(GamePlayer player, int level);
}
