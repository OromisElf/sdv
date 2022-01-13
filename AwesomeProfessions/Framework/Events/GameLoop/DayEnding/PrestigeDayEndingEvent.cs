﻿using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

internal class PrestigeDayEndingEvent : DayEndingEvent
{
    public PerScreen<Queue<SkillType>> SkillsToReset { get; } = new();

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        while (SkillsToReset.Value.Any()) Game1.player.ResetSkill(SkillsToReset.Value.Dequeue());
        ModEntry.State.Value.UsedDogStatueToday = false;
        Disable();
    }
}