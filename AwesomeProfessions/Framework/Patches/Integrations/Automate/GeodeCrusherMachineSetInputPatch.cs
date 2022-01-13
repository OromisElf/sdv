﻿using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

[UsedImplicitly]
internal class GeodeCrusherMachineSetInputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GeodeCrusherMachineSetInputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType()
                .MethodNamed("SetInput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply Gemologist effects to automated Geode Crusher.</summary>
    [HarmonyPostfix]
    private static void GeodeCrusherMachineSetInputPostfix(object __instance)
    {
        if (__instance is null) return;

        var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
        if (machine?.heldObject.Value is null) return;

        var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!who.HasProfession("Gemologist") ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        machine.heldObject.Value.Quality = Utility.Professions.GetGemologistMineralQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        ModData.Increment<uint>(DataField.GemologistMineralsCollected, who);
    }

    #endregion harmony patches
}