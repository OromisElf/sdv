﻿namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class AdventurerGuildGilPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AdventurerGuildGilPatcher"/> class.</summary>
    internal AdventurerGuildGilPatcher()
    {
        this.Target = this.RequireMethod<AdventureGuild>("gil");
    }

    #region harmony patches

    /// <summary>Update virtue progress.</summary>
    [HarmonyPostfix]
    private static void AdventurerGuildGilPostfix()
    {
        WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Valor);
    }

    #endregion harmony patches
}
