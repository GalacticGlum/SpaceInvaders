/*
 * Author: AUTHOR
 * File Name: UfoController.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/08/2019
 * Modified Date: 02/24/2019
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;
using Random = SpaceInvaders.Engine.Random;

namespace SpaceInvaders
{
    public class UfoController
    {
        private const float MinimumSpawnTime = 3;
        private const float MaximumSpawnTime = 20;
        private const int HorizontalSpeed = 100;
        private const int VerticalSpawnPadding = 10;

        private readonly Texture2D ufoTexture;

        private float timeToSpawn;

        private RectangleF BoundingRectangle;
        private int movementDirection;
        private bool isUfoActive;

        public UfoController()
        {
            timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);

            ufoTexture = MainGame.Context.MainTextureAtlas["ufo"];
            BoundingRectangle = new RectangleF(0, 0, ufoTexture.Width * MainGame.ResolutionScale,
                ufoTexture.Height * MainGame.ResolutionScale);
        }

        public void Update(float deltaTime)
        {
            float ufoWidth = ufoTexture.Width * MainGame.ResolutionScale;

            if (isUfoActive)
            {
                BoundingRectangle.X += HorizontalSpeed * movementDirection * deltaTime;

                // Check if the UFO has gone out of bounds
                if (movementDirection == 1 && BoundingRectangle.X - ufoWidth * MainGame.ResolutionScale > MainGame.GameScreenWidth ||
                    movementDirection == -1 && BoundingRectangle.X < -ufoWidth)
                {
                    Destroy();
                }
            }
            else
            {
                timeToSpawn -= deltaTime;
                if (!(timeToSpawn <= 0)) return;

                // A 50% chance to spawn on the left side.
                bool left = Random.Value() > 0.5f;
                Console.WriteLine(left);
                movementDirection = left ? 1 : -1;
                isUfoActive = true;

                float positionY = MainGame.TopVerticalBoundary + ufoTexture.Height * MainGame.ResolutionScale + VerticalSpawnPadding;
                float positionX = left ? -ufoWidth : MainGame.GameScreenWidth + ufoWidth;
                BoundingRectangle.Position = new Vector2(positionX, positionY);
                timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);
                Console.WriteLine(timeToSpawn);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isUfoActive) return;

            spriteBatch.Draw(ufoTexture, BoundingRectangle.Position, null, Color.Red, 0, Vector2.Zero, 
                MainGame.ResolutionScale, SpriteEffects.None, 0.5f);
        }

        public void Destroy()
        {
            isUfoActive = false;
        }

        public bool Intersects(RectangleF rectangle) => isUfoActive && BoundingRectangle.Intersects(rectangle);
    }
}
