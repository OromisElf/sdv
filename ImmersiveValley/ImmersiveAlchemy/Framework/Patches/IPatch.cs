﻿namespace DaLion.Stardew.Alchemy.Framework.Patches;

#region using directives

using HarmonyLib;

#endregion using directives

/// <summary>Interface for Harmony patch classes.</summary>
internal interface IPatch
{
    /// <summary>Apply internally-defined Harmony patches.</summary>
    /// <param name="harmony">The Harmony instance for this mod.</param>
    void Apply(Harmony harmony);
}