﻿namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateInputUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateInputUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateInputUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.Professions.SpecialActivationKey.IsDown())
        {
            Game1.player.Get_Ultimate()!.UpdateInput();
        }
    }
}