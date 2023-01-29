using EpicLoot;
using HarmonyLib;
using System;

namespace Dark_Age_of_Valheim.EpicLoot.MagicItemEffects;

[HarmonyPatch(typeof(Humanoid))]
public class AddStatBonuses
{
    /*
     * 
     * @TODO: Add plus to mixed skills or all skills. e.g +10 to Strength and Dexterity
     * @TODO: Will need to refactor to work with buff potions and stats. Probably be a separate buff bonus for equipment vs spells/pots
     * Calculate buff effects for each attribute/parameter on equipment.
     * 
     */
    [HarmonyPriority(Priority.VeryLow)]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Humanoid.UpdateEquipmentStatusEffects))]
    public static void updateStatBuffsOnItemEquip(Humanoid __instance)
    {
        Player player = __instance as Player;
        if (!player) return;
        float strengthBonus = player.GetTotalActiveMagicEffectValue(MagicEffectTypeExtended.AddStrengthBonus);
        float agilityBonus = player.GetTotalActiveMagicEffectValue(MagicEffectTypeExtended.AddAgilityBonus);
        float intellectBonus = player.GetTotalActiveMagicEffectValue(MagicEffectTypeExtended.AddIntellectBonus);
        float bodyBonus = player.GetTotalActiveMagicEffectValue(MagicEffectTypeExtended.AddBodyBonus);

        GamePlayer.Instance.buffedStrength = Convert.ToInt32(strengthBonus);
        GamePlayer.Instance.buffedAgility = Convert.ToInt32(agilityBonus);
        GamePlayer.Instance.buffedIntellect = Convert.ToInt32(intellectBonus);
        GamePlayer.Instance.buffedBody = Convert.ToInt32(bodyBonus);
    }
}