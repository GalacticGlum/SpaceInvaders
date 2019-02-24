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
using Random = SpaceInvaders.Engine.Random;

namespace SpaceInvaders
{
    public class UfoController
    {
        private const float MinimumSpawnTime = 3;
        private const float MaximumSpawnTime = 20;
        private const int HorizontalSpeed = 100;
        private const int VerticalSpawnPadding = 10;

        private float timeToSpawn;

        private Vector2 currentPosition;
        private int movementDirection;
        private bool isUfoActive;

        public UfoController()
        {
            timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);
        }

        public void Update(float deltaTime)
        {
            Texture2D ufoTexture = MainGame.Context.MainTextureAtlas["ufo"];
            float ufoWidth = ufoTexture.Width * MainGame.ResolutionScale;

            if (isUfoActive)
            {
                currentPosition += new Vector2(HorizontalSpeed * movementDirection, 0) * deltaTime;

                // Check if the UFO has gone out of bounds
                if (movementDirection == 1 && currentPosition.X - ufoWidth * MainGame.ResolutionScale > MainGame.GameScreenWidth ||
                    movementDirection == -1 && currentPosition.X < -ufoWidth)
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
                currentPosition = new Vector2(positionX, positionY);
                timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);
                Console.WriteLine(timeToSpawn);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isUfoActive) return;

            Texture2D ufoTexture = MainGame.Context.MainTextureAtlas["ufo"];
            spriteBatch.Draw(ufoTexture, currentPosition, null, Color.Red, 0, Vector2.Zero, 
                MainGame.ResolutionScale, SpriteEffects.None, 0.5f);
        }

        public void Destroy()
        {
            isUfoActive = false;
        }
    }
}
