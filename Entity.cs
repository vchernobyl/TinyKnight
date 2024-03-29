﻿using TinyKnight.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TinyKnight
{
    public class Entity
    {
        public enum State
        {
            Active,
            Paused,
            Dead,
        }

        public Level Level => GameplayScreen.Level;

        // Coordinates within the grid.
        public int CX = 0;
        public int CY = 0;

        // Position within the cell itself, within range (0, 1).
        public float XR = .5f;
        public float YR = 1f;

        // Resulting coordinates.
        public float XX, YY;

        // Movement.
        public float DX = 0f;
        public float DY = 0f;

        public float Radius = Level.CellSize / 2;

        public State EntityState = State.Active;

        public float FrictionX = .9f;
        public float FrictionY = .9f;
        public float Gravity = .05f;

        public Mask Category = Mask.Default;
        public Mask Collisions = Mask.All;

        public Vector2 Position
        {
            get => new Vector2(XX, YY);
            set => SetCoordinates(value.X, value.Y);
        }

        public bool IsFlashing => flashDuration > .0;

        public Color FlashColor { get; private set; }

        public readonly GameplayScreen GameplayScreen;
        public Sprite? Sprite;

        private double flashDuration = .0;

        private readonly List<Entity> prevFrameCollisions = new List<Entity>();

        public readonly int UpdateOrder;

        public Entity(GameplayScreen gameplayScreen, int updateOrder = 0)
        {
            this.GameplayScreen = gameplayScreen;
            this.UpdateOrder = updateOrder;
        }

        public void SetCoordinates(float x, float y)
        {
            XX = x;
            YY = y;
            CX = (int)(XX / Level.CellSize);
            CY = (int)(YY / Level.CellSize);
            XR = (XX - CX * Level.CellSize) / Level.CellSize;
            YR = (YY - CY * Level.CellSize) / Level.CellSize;
        }

        public bool Overlaps(Entity other)
        {
            var maxDist = Radius + other.Radius;
            var distSqr = (other.XX - XX) * (other.XX - XX) + (other.YY - YY) * (other.YY - YY);
            return distSqr <= maxDist * maxDist;
        }

        public void Flash(double duration, Color color)
        {
            flashDuration = duration;
            FlashColor = color;
        }

        public void Flash(double duration)
        {
            Flash(duration, Color.White);
        }

        public void Destroy()
        {
            EntityState = State.Dead;
        }

        public virtual void OnEntityCollisionEnter(Entity other) { }

        public virtual void OnEntityCollisionStay(Entity other) { }

        public virtual void OnEntityCollisionExit(Entity other) { }

        public virtual void OnLevelCollision(Vector2 normal) { }

        public virtual void OnDestroy() { }

        // This update should be called by entities.
        public virtual void Update(GameTime gameTime) { }

        public virtual void HandleInput(InputState input) { }

        // This should only be called by the underlying game loop.
        // NOT to be used by entities directly.
        public void EntityUpdate(GameTime gameTime)
        {
            Update(gameTime);
            Sprite?.Update(gameTime);

            flashDuration = Math.Max(.0, flashDuration - gameTime.ElapsedGameTime.TotalSeconds);

            // Check for collisions with other entities.
            var copy = new List<Entity>(GameplayScreen.AllEntities);
            foreach (var other in copy)
            {
                if (this == other)
                    continue;

                if (Collisions.HasFlag(other.Category) && Overlaps(other))
                {
                    if (prevFrameCollisions.Contains(other))
                    {
                        OnEntityCollisionStay(other);
                    }
                    else
                    {
                        prevFrameCollisions.Add(other);
                        OnEntityCollisionEnter(other);
                    }
                }
                else
                {
                    if (prevFrameCollisions.Contains(other))
                    {
                        prevFrameCollisions.Remove(other);
                        OnEntityCollisionExit(other);
                    }
                }
            }

            XR += DX;
            DX *= FrictionX;

            // Right side collision.
            if (Collisions.HasFlag(Mask.Level) && Level.HasCollision(CX + 1, CY) && XR >= .7f)
            {
                XR = .7f;
                DX = 0f;
                OnLevelCollision(-Vector2.UnitX);
            }

            // Left side collision.
            if (Collisions.HasFlag(Mask.Level) && Level.HasCollision(CX - 1, CY) && XR <= .3f)
            {
                XR = .3f;
                DX = 0f;
                OnLevelCollision(Vector2.UnitX);
            }

            while (XR > 1) { XR--; CX++; }
            while (XR < 0) { XR++; CX--; }

            YR += DY;
            DY += Gravity;
            DY *= FrictionY;

            // Top collision.
            if (Collisions.HasFlag(Mask.Level) && Level.HasCollision(CX, CY - 1) && YR <= .3f)
            {
                DY = .05f;
                YR = .3f;
                OnLevelCollision(Vector2.UnitY);
            }

            // Bottom collision.
            if (Collisions.HasFlag(Mask.Level) && Level.HasCollision(CX, CY + 1) && YR >= .5f)
            {
                DY = 0f;
                YR = .5f;
                OnLevelCollision(-Vector2.UnitY);
            }

            while (YR > 1) { CY++; YR--; }
            while (YR < 0) { CY--; YR++; }

            XX = (int)((CX + XR) * Level.CellSize);
            YY = (int)((CY + YR) * Level.CellSize);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite?.Draw(Position, spriteBatch);

            if (DebugInfo.ShowEntityColliders)
                DebugRenderer.AddCircle(Position, Radius, Color.LimeGreen);
        }
    }
}
