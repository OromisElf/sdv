﻿namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawPatcher"/> class.</summary>
    internal ToolDrawPatcher()
    {
        this.Target = this.RequireMethod<Tool>("draw");
    }

    #region harmony patches

    /// <summary>Hide affected tiles overlay.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawPrefix()
    {
        return !ModEntry.Config.Tools.HideAffectedTiles;
    }

    #endregion harmony patches
}
