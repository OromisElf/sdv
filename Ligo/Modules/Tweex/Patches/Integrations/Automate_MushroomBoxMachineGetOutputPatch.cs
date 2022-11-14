﻿namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Tweex.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class MushroomBoxMachineGetOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MushroomBoxMachineGetOutputPatcher"/> class.</summary>
    internal MushroomBoxMachineGetOutputPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality.</summary>
    [HarmonyPrefix]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            var machine = ModEntry.Reflector
                .GetUnboundPropertyGetter<object, SObject>(__instance, "Machine")
                .Invoke(__instance);
            if (machine.heldObject.Value is not { } held)
            {
                return;
            }

            var owner = ModEntry.Config.EnableProfessions && !ModEntry.Config.Professions.LaxOwnershipRequirements
                ? machine.GetOwner()
                : Game1.player;
            if (!owner.professions.Contains(Farmer.botanist))
            {
                held.Quality = held.GetQualityFromAge();
            }
            else if (ModEntry.Config.EnableProfessions)
            {
                held.Quality = Math.Max(owner.GetEcologistForageQuality(), held.Quality);
            }
            else
            {
                held.Quality = SObject.bestQuality;
            }

            if (ModEntry.Config.Tweex.MushroomBoxesRewardExp)
            {
                Game1.player.gainExperience(Farmer.foragingSkill, 1);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
