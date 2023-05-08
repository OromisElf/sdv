﻿namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.Events;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Steadfast enchantment crit. to damage conversion + Artful parry crit. bonus.</summary>
    [HarmonyPrefix]
    private static void GameLocationDamageMonsterPrefix(
        ref int minDamage,
        ref int maxDamage,
        ref float critChance,
        float critMultiplier,
        Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon)
        {
            return;
        }

        if (weapon.hasEnchantmentOfType<MeleeArtfulEnchantment>() && EnchantmentsModule.State.DidArtfulParry)
        {
            critChance += 1f;
        }
        else if (weapon.hasEnchantmentOfType<SteadfastEnchantment>())
        {
            minDamage += (int)(critChance * 100 * critMultiplier);
            maxDamage += (int)(critChance * 100 * critMultiplier);
            critChance = 0f;
        }
    }

    /// <summary>Reset Artful parry crit. chance.</summary>
    [HarmonyPostfix]
    private static void GameLocationDamageMonsterTranspiler()
    {
        if (!EnchantmentsModule.State.DidArtfulParry)
        {
            return;
        }

        EventManager.Disable<ArtfulParryUpdateTickedEvent>();
        EnchantmentsModule.State.DidArtfulParry = false;
    }

    #endregion harmony patches
}