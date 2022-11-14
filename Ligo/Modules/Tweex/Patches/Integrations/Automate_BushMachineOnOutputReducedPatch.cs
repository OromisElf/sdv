﻿namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class BushMachineOnOutputReducedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushMachineOnOutputReducedPatcher"/> class.</summary>
    internal BushMachineOnOutputReducedPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine"
            .ToType()
            .RequireMethod("OnOutputReduced");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    [HarmonyPostfix]
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (!ModEntry.Config.Tweex.BerryBushesRewardExp)
        {
            return;
        }

        var machine = ModEntry.Reflector
            .GetUnboundPropertyGetter<object, Bush>(__instance, "Machine")
            .Invoke(__instance);
        if (machine.size.Value >= Bush.greenTeaBush)
        {
            return;
        }

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
