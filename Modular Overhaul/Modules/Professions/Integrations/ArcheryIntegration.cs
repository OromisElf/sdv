﻿namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.Archery;

#endregion using directives

[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class ArcheryIntegration : ModIntegration<ArcheryIntegration, IArcheryApi>
{
    /// <summary>Initializes a new instance of the <see cref="ArcheryIntegration"/> class.</summary>
    internal ArcheryIntegration()
        : base("PeacefulEnd.Archery", "Archery", "1.2.0", ModHelper.ModRegistry)
    {
    }
}
