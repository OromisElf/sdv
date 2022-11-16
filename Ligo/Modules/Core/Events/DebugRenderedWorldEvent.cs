﻿namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedWorldEvent : RenderedWorldEvent
{
    private readonly Texture2D _pixel;

    /// <summary>Initializes a new instance of the <see cref="DebugRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
        this._pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        this._pixel.SetData(new[] { Color.White });
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (Globals.DebugCursorPosition is null)
        {
            return;
        }

        var bb = default(Rectangle);
        if (Game1.currentLocation.Objects.TryGetValue(Globals.DebugCursorPosition.Tile, out var o))
        {
            bb = o.getBoundingBox(o.TileLocation);
        }
        else
        {
            foreach (var c in Game1.currentLocation.characters.Cast<Character>()
                         .Concat(Game1.currentLocation.farmers))
            {
                var tileLocation = c.getTileLocation();
                if (tileLocation != Globals.DebugCursorPosition.Tile)
                {
                    continue;
                }

                bb = c.GetBoundingBox();
                break;
            }
        }

        bb.X -= Game1.viewport.X;
        bb.Y -= Game1.viewport.Y;
        bb.DrawBorder(this._pixel, 3, Color.Red, e.SpriteBatch);

        var (x, y) = Game1.player.getTileLocation() * Game1.tileSize;
        var facingBox = (FacingDirection)Game1.player.FacingDirection switch
        {
            FacingDirection.Up => new Rectangle((int)x, (int)y - Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Right => new Rectangle((int)x + Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            FacingDirection.Down => new Rectangle((int)x, (int)y + Game1.tileSize, Game1.tileSize, Game1.tileSize),
            FacingDirection.Left => new Rectangle((int)x - Game1.tileSize, (int)y, Game1.tileSize, Game1.tileSize),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Rectangle>(
                (FacingDirection)Game1.player.FacingDirection),
        };

        facingBox.X -= Game1.viewport.X;
        facingBox.Y -= Game1.viewport.Y;
        facingBox.DrawBorder(this._pixel, 3, Color.Red, e.SpriteBatch);
    }
}