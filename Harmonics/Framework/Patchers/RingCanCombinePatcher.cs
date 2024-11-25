﻿namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCanCombinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingCanCombinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal RingCanCombinePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.CanCombine));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Allows feeding up to four gemstone rings into an Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCanCombinePrefix(Ring __instance, ref bool __result, Ring ring)
    {
        if (__instance.QualifiedItemId == QualifiedObjectIds.IridiumBand ||
            ring.QualifiedItemId == QualifiedObjectIds.IridiumBand ||
            ring.QualifiedItemId == InfinityBandId)
        {
            return false; // don't run original logic
        }

        if (__instance.QualifiedItemId != $"(O){InfinityBandId}")
        {
            return true; // run original logic
        }

        __result = Gemstone.TryFromRing(ring.QualifiedItemId, out _) &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}