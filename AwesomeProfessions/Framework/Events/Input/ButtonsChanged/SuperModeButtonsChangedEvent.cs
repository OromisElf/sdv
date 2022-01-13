﻿using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Input;

internal class SuperModeButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        ModEntry.State.Value.SuperMode.ReceiveInput();
    }
}