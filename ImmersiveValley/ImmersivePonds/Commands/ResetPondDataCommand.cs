﻿namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Data;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ResetPondDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ResetPondDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "reset_data";

    /// <inheritdoc />
    public override string Documentation => "Reset custom mod data of nearest pond.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (!ponds.Any())
        {
            Log.W("You don't own any Fish Ponds.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding(out _, ponds);
        if (nearest is null)
        {
            Log.W("There are no ponds nearby.");
            return;
        }

        ModDataIO.WriteData(nearest, "FishQualities", null);
        ModDataIO.WriteData(nearest, "FamilyQualities", null);
        ModDataIO.WriteData(nearest, "FamilyLivingHere", null);
        ModDataIO.WriteData(nearest, "DaysEmpty", 0.ToString());
        ModDataIO.WriteData(nearest, "SeaweedLivingHere", null);
        ModDataIO.WriteData(nearest, "GreenAlgaeLivingHere", null);
        ModDataIO.WriteData(nearest, "WhiteAlgaeLivingHere", null);
        ModDataIO.WriteData(nearest, "CheckedToday", null);
        ModDataIO.WriteData(nearest, "ItemsHeld", null);
    }
}