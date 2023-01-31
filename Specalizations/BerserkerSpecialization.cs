using System;
using static Dark_Age_of_Valheim.GlobalConstants;

namespace Dark_Age_of_Valheim.Specalizations;

[Specialization((byte)eSpecialization.Berserker, "Berserker", "Blank Description", 25, 50, 10)]
class BerserkerSpecialization : SpecializationBase
{
    public string GetTitle(GamePlayer player, int level)
    {
        return "";
    }
}
