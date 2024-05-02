﻿namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        State.ScavengerHunt ??= new ScavengerHunt();
        if (!State.ScavengerHunt.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = State.ScavengerHunt.TreasureTile.Value;

        // track target
        HudPointer.Instance.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        if (Game1.player.DistanceTo(treasureTile) <= Math.Pow(Config.ScavengerDetectionDistance, 2))
        {
            HudPointer.Instance.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}