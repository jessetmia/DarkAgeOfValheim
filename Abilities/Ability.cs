using EpicLoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dark_Age_of_Valheim.Abilities
{
    [Serializable]
    public class Ability
    {
        public string name { get; set; }
        public string description { get; set; }

        //Type of Ability e.g. IncreaseStaminaRegen. Uses EpicLoot MagicEffects to handle buffs
        public string abilityType { get; set; }

        //The amount of improvement of the abilityType
        public int abilityValue { get; set; }

        public int levelRequired { get; set; }

        public int duration { get; set; }
        
        public bool isPassive { get; set; }

        public string? icon { get; set; }

    }
}
