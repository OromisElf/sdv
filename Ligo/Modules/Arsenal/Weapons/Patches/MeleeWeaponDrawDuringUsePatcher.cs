﻿namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawDuringUsePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawDuringUsePatcher"/> class.</summary>
    internal MeleeWeaponDrawDuringUsePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(
            nameof(MeleeWeapon.drawDuringUse),
            new[]
            {
                typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), typeof(Farmer), typeof(Rectangle),
                typeof(int), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw weapon during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawDuringUsePrefix(
        Vector2 ___center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer f,
        Rectangle sourceRect,
        int type,
        bool isOnSpecial)
    {
        if (type == MeleeWeapon.stabbingSword && isOnSpecial)
        {
            DrawDuringStabbySwordLunge(
                ___center,
                frameOfFarmerAnimation,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
            return false; // don't run original logic
        }

        if (ModEntry.State.Arsenal.ComboHitStep <= ComboHitStep.FirstHit)
        {
            return true; // run original logic
        }

        if (type == MeleeWeapon.club)
        {
            DrawDuringClubCombo(
                frameOfFarmerAnimation,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
        }
        else
        {
            DrawDuringSwordCombo(
                ___center,
                facingDirection,
                spriteBatch,
                playerPosition,
                f,
                sourceRect);
        }

        return false;  // don't run original logic
    }

    #endregion harmony patches

    private static void DrawDuringStabbySwordLunge(
        Vector2 center,
        int frameOfFarmerAnimation,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer farmer,
        Rectangle sourceRectangle)
    {
        frameOfFarmerAnimation %= 2;
        switch (facingDirection)
        {
            case Game1.up:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 48f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32) / 10000f));
                        break;
                }

                break;

            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 24f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            case Game1.down:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 12f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 21f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                }

                break;

            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }
    }

    private static void DrawDuringSwordCombo(
        Vector2 center,
        int facingDirection,
        SpriteBatch spriteBatch,
        Vector2 playerPosition,
        Farmer farmer,
        Rectangle sourceRectangle)
    {
        var frame = farmer.FarmerSprite.CurrentFrame;
        switch (facingDirection)
        {
            case Game1.up:
                switch (frame)
                {
                    case 36:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 37:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 38:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 39:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 40:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                    case 41:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 32 - 8) / 10000f));
                        break;
                }

                break;

            case Game1.right:
                switch (frame)
                {
                    case 30:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 40f, playerPosition.Y - 64f + 8f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 31:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 56f, playerPosition.Y - 64f + 28f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 32:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 33:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 34:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 28f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 5f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 35:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            case Game1.down:
                switch (frame)
                {
                    case 24:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 25:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 26:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 40f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 27:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 28:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 29:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 12f, playerPosition.Y),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * 9f / 8f,
                            center,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                }

                break;

            case Game1.left:
                switch (frame)
                {
                    case 30:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f, playerPosition.Y - 64f - 16f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 31:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 48f, playerPosition.Y - 64f + 20f),
                            sourceRectangle,
                            Color.White,
                            0f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 32:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 64f + 32f, playerPosition.Y + 16f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() - 1) / 10000f));
                        break;
                    case 33:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f),
                            sourceRectangle,
                            Color.White,
                            -(float)Math.PI / 2f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 34:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -5f / 8f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 35:
                        spriteBatch.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f),
                            sourceRectangle,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            center,
                            4f,
                            SpriteEffects.FlipHorizontally,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }
    }

    private static void DrawDuringClubCombo(
       int frameOfFarmerAnimation,
       int facingDirection,
       SpriteBatch b,
       Vector2 playerPosition,
       Farmer farmer,
       Rectangle sourceRect)
    {
        switch (facingDirection)
        {
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 6:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 32f - 12f, playerPosition.Y - 80f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -3f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 7:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f, playerPosition.Y - 64f - 48f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 8:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 128f - 16f, playerPosition.Y - 64f - 12f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 9:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 72f, playerPosition.Y - 64f + 16f - 32f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 10:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f, playerPosition.Y - 64f + 16f - 16f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 11:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f - 12f, playerPosition.Y - 64f + 16f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 12:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f - 16f, playerPosition.Y - 64f + 40f - 8f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 13:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 96f - 8f, playerPosition.Y + 40f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 5f / 16f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 6:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f - 4f + 8f, playerPosition.Y - 56f - 64f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 7:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -5f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 8:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -7f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 9:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 32f - 4f, playerPosition.Y + 8f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -3f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 10:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f - 24f, playerPosition.Y + 64f + 12f - 64f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 11f / 8f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 11:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 20f, playerPosition.Y + 64f + 40f - 64f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 5f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 12:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f, playerPosition.Y + 64f + 56f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 5f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                    case 13:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 8f, playerPosition.Y + 64f + 64f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 19f / 16f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 64) / 10000f));
                        break;
                }

                break;

            default:
                switch (frameOfFarmerAnimation)
                {
                    case 6:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 24f, playerPosition.Y - 21f - 8f - 64f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 7:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f - 64f + 4f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 8:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 20f - 64f),
                            sourceRect,
                            Color.White,
                            -(float)Math.PI / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 9:
                        if (facingDirection == Game1.down)
                        {
                            b.Draw(
                                Tool.weaponsTexture,
                                new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f),
                                sourceRect,
                                Color.White,
                                (float)Math.PI * -5f / 4f,
                                Vector2.Zero,
                                4f,
                                SpriteEffects.None,
                                Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        }
                        else
                        {
                            b.Draw(
                                Tool.weaponsTexture,
                                new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 32f - 64f),
                                sourceRect,
                                Color.White,
                                -(float)Math.PI / 4f,
                                Vector2.Zero,
                                4f,
                                SpriteEffects.None,
                                Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        }

                        break;
                    case 10 when facingDirection == Game1.down:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * -5f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 11 when facingDirection == Game1.down:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f - 20f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 12 when facingDirection == Game1.down:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 54f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                    case 13 when facingDirection == Game1.down:
                        b.Draw(
                            Tool.weaponsTexture,
                            new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 58f),
                            sourceRect,
                            Color.White,
                            (float)Math.PI * 3f / 4f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            Math.Max(0f, (farmer.getStandingY() + 32) / 10000f));
                        break;
                }

                if (facingDirection == Game1.up)
                {
                    farmer.FarmerRenderer.draw(
                        b,
                        farmer.FarmerSprite,
                        farmer.FarmerSprite.SourceRect,
                        farmer.getLocalPosition(Game1.viewport),
                        new Vector2(0f, ((farmer.yOffset + 128f - (farmer.GetBoundingBox().Height / 2)) / 4f) + 4f),
                        Math.Max(0f, (farmer.getStandingY() / 10000f) + 0.0099f),
                        Color.White,
                        0f,
                        farmer);
                }

                break;
        }
    }
}