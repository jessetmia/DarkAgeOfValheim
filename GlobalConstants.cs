using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dark_Age_of_Valheim
{
    public class GlobalConstants
    {
        /// <summary>
        /// Holds all character classes
        /// </summary>
        public enum eSpecialization : Byte
        {
            Unknown = 0,
            Berserker = 1,
            Bonedancer = 2,
            Healer = 3,
            Hunter = 4,
            Runemaster = 5,
            Savage = 6,
            Shadowblade = 7,
            Shaman = 8,
            Skald = 9,
            Spiritmaster = 10,
            Thane = 11,
            Valkyrie = 12,
            Warlock = 13,
            Warrior = 14
        }

    }
}
