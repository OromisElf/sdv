﻿namespace DaLion.Overhaul.Modules.Professions.Events.Custom;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProfessionFirstSecondUpdateTickedEvent : FirstSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionFirstSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionFirstSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnFirstSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        this.Manager.Enable<LateLoadOneSecondUpdateTickedEvent>();
    }
}
