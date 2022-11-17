﻿namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeDayUpdatePatcher"/> class.</summary>
    internal FruitTreeDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
        this.Prefix!.before = new[] { "DaLion.Professions", "atravita.MoreFertilizers" };
        this.Postfix!.after = new[] { "DaLion.Professions", "atravita.MoreFertilizers" };
    }

    #region harmony patches

    /// <summary>Record growth stage.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.Professions", "atravita.MoreFertilizers")]
    private static void FruitTreeDayUpdatePrefix(FruitTree __instance, ref (int DaysUntilMature, int GrowthStage) __state)
    {
        __state.DaysUntilMature = __instance.daysUntilMature.Value;
        __state.GrowthStage = __instance.growthStage.Value;
    }

    /// <summary>Undo growth during winter.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Professions", "atravita.MoreFertilizers")]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance, (int DaysUntilMature, int GrowthStage) __state)
    {
        if (!ModEntry.Config.Tweex.PreventFruitTreeGrowthInWinter || __instance.growthStage.Value >= FruitTree.treeStage ||
            !Game1.IsWinter || __instance.currentLocation.IsGreenhouse ||
            __instance.Read<int>("FruitTree", modId: "atravita.MoreFertilizers") > 0)
        {
            return;
        }

        __instance.daysUntilMature.Value = __state.DaysUntilMature;
        __instance.growthStage.Value = __state.GrowthStage;
    }

    #endregion harmony patches
}
