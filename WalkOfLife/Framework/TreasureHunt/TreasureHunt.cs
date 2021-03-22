﻿using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal abstract class TreasureHunt
	{
		public static Vector2? TreasureTile { get; protected set; } = null;
		protected static uint elapsed = 0;
		
		private protected ProfessionsData _data;
		private protected EventManager _manager;
		protected readonly Random random = new Random(Guid.NewGuid().GetHashCode());

		private readonly uint _timeLimit;
		private readonly double _baseTriggerChance;
		private double _accumulatedBonus = 1.0;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		internal TreasureHunt(ProfessionsConfig config, ProfessionsData data, EventManager manager)
		{
			_baseTriggerChance = config.ChanceToStartTreasureHunt;
			_timeLimit = config.TreasureHuntTimeLimitSeconds;
			_data = data;
			_manager = manager;
		}

		/// <summary>Check for completion or failure on every update tick.</summary>
		/// <param name="ticks">The number of ticks elapsed since the game started.</param>
		internal void Update(uint ticks)
		{
			if (ticks % 60 == 0 && ++elapsed > _timeLimit) Fail();

			CheckForCompletion();
		}

		/// <summary>Start a new treasure hunt or adjust the odds for the next attempt.</summary>
		protected bool TryStartNewHunt()
		{
			if (random.NextDouble() > _baseTriggerChance * _accumulatedBonus)
			{
				_accumulatedBonus *= 1.0 + Game1.player.DailyLuck;
				return false;
			}

			_accumulatedBonus = 1.0;
			return true;
		}

		/// <summary>Try to start a new hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		internal abstract void TryStartNewHunt(GameLocation location);

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected abstract void CheckForCompletion();

		/// <summary>End the hunt unsuccessfully.</summary>
		protected abstract void Fail();

		/// <summary>Reset treasure tile and unsubscribe treasure hunt update event.</summary>
		protected abstract void End();
	}
}