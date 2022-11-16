﻿namespace DaLion.Ligo.Modules.Core.Configs;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the Ponds menu.</summary>
    private void RegisterPonds()
    {
        this._configMenu
            .AddPage(LigoModule.Ponds.Namespace, () => "Pond Settings")
            .AddNumberField(
                () => "Roe Production Chance Multiplier",
                () => "Multiplies a fish's base chance to produce roe each day.",
                config => config.Ponds.RoeProductionChanceMultiplier,
                (config, value) => config.Ponds.RoeProductionChanceMultiplier = value,
                0.1f,
                2f);
    }
}