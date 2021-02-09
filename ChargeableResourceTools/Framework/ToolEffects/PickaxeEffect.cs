﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using TheLion.AwesomeTools.Framework.Configs;
using TheLion.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeTools.Framework.ToolEffects
{
	/// <summary>Applies Pickaxe-specific effects.</summary>
	internal class PickaxeEffect : BaseEffect
	{
		public PickaxeConfig Config { get; }

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The effect settings.</param>
		public PickaxeEffect(PickaxeConfig config, IModRegistry modRegistry)
			: base(modRegistry)
		{
			Config = config;
		}

		/// <summary>Apply the tool effect to the given tile.</summary>
		/// <param name="tile">The tile to modify.</param>
		/// <param name="tileObj">The object on the tile.</param>
		/// <param name="tileFeature">The feature on the tile.</param>
		/// <param name="tool">The tool selected by the player.</param>
		/// <param name="location">The current location.</param>
		/// <param name="who">The current player.</param>
		public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who)
		{
			// clear debris
			if (Config.ClearDebris && (IsStone(tileObj) || IsWeed(tileObj)))
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// break mine containers
			if (Config.BreakMineContainers && tileObj != null)
			{
				return TryBreakContainer(tile, tileObj, tool, location);
			}

			// clear placed objects
			if (Config.ClearObjects && tileObj != null)
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// clear placed paths & flooring
			if (Config.ClearFlooring && tileFeature is Flooring)
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// clear bushes
			if (Config.ClearBushes && tileFeature is Bush bush)
				return UseToolOnTile(tool, tile, who, location);

			// handle dirt
			if (tileFeature is HoeDirt dirt)
			{
				// clear tilled dirt
				if (dirt.crop == null && Config.ClearDirt)
				{
					return UseToolOnTile(tool, tile, who, location);
				}
				// clear crops
				else if (dirt.crop != null)
				{
					if (Config.ClearDeadCrops && dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
					else if (Config.ClearLiveCrops && !dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
				}
			}

			// clear boulders / meteorites
			if (Config.BreakBouldersAndMeteorites)
			{
				ResourceClump clump = GetResourceClumpCoveringTile(location, tile, who, out var applyTool);
				if (clump != null && clump.parentSheetIndex.Value.IsIn(ResourceClump.boulderIndex, ResourceClump.meteoriteIndex))
				{
					return applyTool(tool);
				}
			}

			// harvest spawned mine objects
			if (Config.HarvestMineSpawns && location is MineShaft && tileObj?.IsSpawnedObject == true && CheckTileAction(location, tile, who))
			{
				CancelAnimation(who, FarmerSprite.harvestItemDown, FarmerSprite.harvestItemLeft, FarmerSprite.harvestItemRight, FarmerSprite.harvestItemUp);
				return true;
			}

			return false;
		}
	}
}