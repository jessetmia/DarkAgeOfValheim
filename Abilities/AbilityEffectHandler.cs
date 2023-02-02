using EpicLoot.MagicItemEffects;
using EpicLoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Dark_Age_of_Valheim.Abilities;

public static class AbilityEffectHandler
{
    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyStaminaRegen))]
    public static void ModifyStaminaRegen(SEMan __instance, ref float staminaMultiplier)
    {
        if (__instance.m_character.IsPlayer())
        {
            if (GamePlayer.Instance.Abilities is null) { return;  }

            Ability? staminaRegen = GamePlayer.Instance.Abilities?.Find(i => i.abilityType == MagicEffectType.ModifyStaminaRegen);
            if (staminaRegen == null) { return; }

            Player player = __instance.m_character as Player;
            float regenValue = (float)staminaRegen.abilityValue;
            staminaMultiplier += regenValue;
        }
    }
}
