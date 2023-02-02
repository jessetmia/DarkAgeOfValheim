using Dark_Age_of_Valheim.Abilities;
using System;
using System.Collections.Generic;

namespace Dark_Age_of_Valheim.Specalizations;

public interface ISpecalization
{
    byte id { get; set; }
    string name { get; set;  }
    string description { get; set;  }

    //Dictionary<int, string> title { get; }

    //Different classes will have more HP depending on spec. 
    int bonusHp { get; set;  }

    //Starting Stamina Bonus
    int bonusStamina { get; set;  }

    //Starting Mana
    int bonusEitr { get; set;  }

    //Custom Skills for the class (dual wield for berserker, stormcalling for thane, etc)
    Skills[]? skillLines { get; set;  }

    Skills[]? acceleratedSkills { get; set;  }

    Ability[]? classAbilities { get; set;  }

    //string getTitle(GamePlayer player, int level);
}
