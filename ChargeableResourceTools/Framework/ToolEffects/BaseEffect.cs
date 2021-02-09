﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeTools.Framework.ToolEffects
{
	/// <summary>Applies base effects shared by multiple tools.</summary>
	internal abstract class BaseEffect
	{
		/// <summary>Whether the Farm Type Manager mod is installed.</summary>
		private readonly bool _hasFarmTypeManager;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The effect settings.</param>
		public BaseEffect(IModRegistry modRegistry)
		{
			_hasFarmTypeManager = modRegistry.IsLoaded("Esca.FarmTypeManager");
		}

		/// <summary>Apply the tool effect to the given tile.</summary>
		/// <param name="tile">The tile to modify.</param>
		/// <param name="tileObj">The object on the tile.</param>
		/// <param name="tileFeature">The feature on the tile.</param>
		/// <param name="tool">The tool selected by the player (if any).</param>
		/// <param name="location">The current location.</param>
		/// <param name="who">The current player.</param>
		public abstract bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who);

		/// <summary>Spreads the effects of Axe and Pickaxe to an area around the player.</summary>
		/// <param name="origin">The center of the shockwave (i.e. the tool's tile location).</param>
		/// <param name="radius">The radius of the shockwave area.</param>
		/// <param name="tool">The tool select by the player.</param>
		/// <param name="location">The current location.</param>
		/// <param name="who">The current player.</param>
		public void SpreadResourceToolEffect(Tool tool, Vector2 origin, int[] radii, GameLocation location, Farmer who)
		{
			int radius = radii[who.toolPower - 1];

			DelayedAction shockwave = new DelayedAction(220, () =>
			{
				ModEntry.IsDoingShockwave = true;
				foreach (Vector2 tile in Utils.GetTilesAround(origin, radius))
				{
					TemporarilyFakeInteraction(() =>
					{
						// face tile to avoid game skipping interaction
						GetRadialAdjacentTile(origin, tile, out Vector2 adjacentTile, out int facingDirection);
						who.Position = adjacentTile * Game1.tileSize;
						who.FacingDirection = facingDirection;

						// apply tool effects
						location.objects.TryGetValue(tile, out SObject tileObj);
						location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
						Apply(tile, tileObj, tileFeature, tool, location, who);
					});
				}
				ModEntry.IsDoingShockwave = false;
			});
			Game1.delayedActions.Add(shockwave);
		}

		/// <summary>Temporarily set up the player to interact with a tile, then return it to the original state.</summary>
		/// <param name="action">The action to perform.</param>
		private void TemporarilyFakeInteraction(Action action)
		{
			Farmer who = Game1.player;

			// save current state
			float stamina = who.stamina;
			Vector2 position = who.Position;
			int facingDirection = who.FacingDirection;
			int currentToolIndex = who.CurrentToolIndex;
			bool canMove = who.canMove; // fix player frozen due to animations when performing an action

			// perform action
			try
			{
				action();
			}
			finally
			{
				// restore previous state
				who.stamina = stamina;
				who.Position = position;
				who.FacingDirection = facingDirection;
				who.CurrentToolIndex = currentToolIndex;
				who.canMove = canMove;
			}
		}

		/// <summary>Use a tool on a tile.</summary>
		/// <param name="tool">The tool to use.</param>
		/// <param name="tile">The tile to affect.</param>
		/// <param name="who">The current player.</param>
		/// <param name="location">The current location.</param>
		/// <returns>Returns <c>true</c> for convenience when implementing tools.</returns>
		protected bool UseToolOnTile(Tool tool, Vector2 tile, Farmer who, GameLocation location)
		{
			// use tool on center of tile
			who.lastClick = GetToolPixelPosition(tile);
			tool.DoFunction(location, (int)who.lastClick.X, (int)who.lastClick.Y, 0, who);
			return true;
		}

		/// <summary>Get the pixel position relative to the top-left corner of the map at which to use a tool.</summary>
		/// <param name="tile">The tile to affect.</param>
		protected Vector2 GetToolPixelPosition(Vector2 tile)
		{
			return (tile * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
		}

		/// <summary>Trigger the player action on the given tile.</summary>
		/// <param name="location">The location for which to trigger an action.</param>
		/// <param name="tile">The tile for which to trigger an action.</param>
		/// <param name="who">The player for which to trigger an action.</param>
		protected bool CheckTileAction(GameLocation location, Vector2 tile, Farmer who)
		{
			return location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, who);
		}

		/// <summary>Get whether a given object is a twig.</summary>
		/// <param name="obj">The world object.</param>
		protected bool IsTwig(SObject obj)
		{
			return obj?.ParentSheetIndex == 294 || obj?.ParentSheetIndex == 295;
		}

		/// <summary>Get whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		protected bool IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}

		/// <summary>Get whether a given object is a weed.</summary>
		/// <param name="obj">The world object.</param>
		protected bool IsWeed(SObject obj)
		{
			return !(obj is Chest) && obj?.Name == "Weeds";
		}

		/// <summary>Get a rectangle representing the tile area in absolute pixels from the map origin.</summary>
		/// <param name="tile">The tile position.</param>
		protected Rectangle GetAbsoluteTileArea(Vector2 tile)
		{
			Vector2 pos = tile * Game1.tileSize;
			return new Rectangle((int)pos.X, (int)pos.Y, Game1.tileSize, Game1.tileSize);
		}

		/// <summary>Get the resource clumps in a given location.</summary>
		/// <param name="location">The location to search.</param>
		private IEnumerable<ResourceClump> GetNormalResourceClumps(GameLocation location)
		{
			IEnumerable<ResourceClump> clumps = location.resourceClumps;

			switch (location)
			{
				case Forest forest when forest.log != null:
					clumps = clumps.Concat(new[] { forest.log });
					break;

				case Woods woods when woods.stumps.Any():
					clumps = clumps.Concat(woods.stumps);
					break;
			}

			return clumps;
		}

		/// <summary>Get the resource clump which covers a given tile, if any.</summary>
		/// <param name="location">The location to check.</param>
		/// <param name="tile">The tile to check.</param>
		protected bool HasResourceClumpCoveringTile(GameLocation location, Vector2 tile)
		{
			return GetResourceClumpCoveringTile(location, tile, null, out _) != null;
		}

		/// <summary>Get the resource clump which covers a given tile, if any.</summary>
		/// <param name="location">The location to check.</param>
		/// <param name="tile">The tile to check.</param>
		/// <param name="who">The current player.</param>
		/// <param name="applyTool">Applies a tool to the resource clump.</param>
		protected ResourceClump GetResourceClumpCoveringTile(GameLocation location, Vector2 tile, Farmer who, out Func<Tool, bool> applyTool)
		{
			Rectangle tileArea = GetAbsoluteTileArea(tile);

			// normal resource clumps
			foreach (ResourceClump clump in GetNormalResourceClumps(location))
			{
				if (clump.getBoundingBox(clump.tile.Value).Intersects(tileArea))
				{
					applyTool = tool => UseToolOnTile(tool, tile, who, location);
					return clump;
				}
			}

			// FarmTypeManager resource clumps
			if (_hasFarmTypeManager)
			{
				foreach (LargeTerrainFeature feature in location.largeTerrainFeatures)
				{
					if (feature.GetType().FullName == "FarmTypeManager.LargeResourceClump" && feature.getBoundingBox(feature.tilePosition.Value).Intersects(tileArea))
					{
						ResourceClump clump = ModEntry.Reflection.GetField<Netcode.NetRef<ResourceClump>>(feature, "Clump").GetValue().Value;
						applyTool = tool => feature.performToolAction(tool, 0, tile, location);
						return clump;
					}
				}
			}

			applyTool = null;
			return null;
		}

		/// <summary>Break open a container using a tool, if applicable.</summary>
		/// <param name="tile">The tile position</param>
		/// <param name="tileObj">The object on the tile.</param>
		/// <param name="tool">The tool selected by the player (if any).</param>
		/// <param name="location">The current location.</param>
		protected bool TryBreakContainer(Vector2 tile, SObject tileObj, Tool tool, GameLocation location)
		{
			if (tileObj is BreakableContainer)
				return tileObj.performToolAction(tool, location);

			if (!tileObj.bigCraftable.Value && tileObj.Name == "SupplyCrate" && !(tileObj is Chest) && tileObj.performToolAction(tool, location))
			{
				tileObj.performRemoveAction(tile, location);
				Game1.currentLocation.Objects.Remove(tile);
				return true;
			}

			return false;
		}

		/// <summary>Get the tilled dirt for a tile, if any.</summary>
		/// <param name="tileFeature">The feature on the tile.</param>
		/// <param name="tileObj">The object on the tile.</param>
		/// <param name="dirt">The tilled dirt found, if any.</param>
		/// <param name="isCoveredByObj">Whether there's an object placed over the tilled dirt.</param>
		/// <param name="pot">The indoor pot containing the dirt, if applicable.</param>
		/// <returns>Returns whether tilled dirt was found.</returns>
		protected bool TryGetHoeDirt(TerrainFeature tileFeature, SObject tileObj, out HoeDirt dirt, out bool isCoveredByObj, out IndoorPot pot)
		{
			// garden pot
			if (tileObj is IndoorPot foundPot)
			{
				pot = foundPot;
				dirt = pot.hoeDirt.Value;
				isCoveredByObj = false;
				return true;
			}

			// regular dirt
			if ((dirt = tileFeature as HoeDirt) != null)
			{
				pot = null;
				isCoveredByObj = tileObj != null;
				return true;
			}

			// none found
			pot = null;
			dirt = null;
			isCoveredByObj = false;
			return false;
		}

		/// <summary>Cancel the current player animation if it matches one of the given IDs.</summary>
		/// <param name="who">The player to change.</param>
		/// <param name="animationIds">The animation IDs to detect.</param>
		protected void CancelAnimation(Farmer who, params int[] animationIds)
		{
			int animationId = ModEntry.Reflection.GetField<int>(who.FarmerSprite, "currentSingleAnimation").GetValue();
			foreach (int id in animationIds)
			{
				if (id == animationId)
				{
					who.completelyStopAnimatingOrDoingAction();
					who.forceCanMove();
					break;
				}
			}
		}

		/// <summary>Get the tile coordinate which is adjacent to the given <paramref name="tile"/> along a radial line from the player.</summary>
		/// <param name="origin">The tile containing the player.</param>
		/// <param name="tile">The tile to face.</param>
		/// <param name="adjacent">The tile radially adjacent to the <paramref name="tile"/>.</param>
		/// <param name="facingDirection">The direction to face.</param>
		protected void GetRadialAdjacentTile(Vector2 origin, Vector2 tile, out Vector2 adjacent, out int facingDirection)
		{
			facingDirection = Utility.getDirectionFromChange(tile, origin);
			adjacent = facingDirection switch
			{
				Game1.up => new Vector2(tile.X, tile.Y + 1),
				Game1.down => new Vector2(tile.X, tile.Y - 1),
				Game1.left => new Vector2(tile.X + 1, tile.Y),
				Game1.right => new Vector2(tile.X - 1, tile.Y),
				_ => tile
			};
		}
	}
}