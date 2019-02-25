/*
 * Author: AUTHOR
 * File Name: UfoController.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/08/2019
 * Modified Date: 02/24/2019
 * Description: The UFO spawner and controller.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Engine;
using Random = SpaceInvaders.Engine.Random;

namespace SpaceInvaders
{
    /// <summary>
    /// The UFO spawner and controller.
    /// </summary>
    public class UfoController
    {
        /// <summary>
        /// The minimum time, in seconds, that it takes to spawn a UFO.
        /// </summary>
        private const float MinimumSpawnTime = 3;

        /// <summary>
        /// The maximum time, in seconds, that it takes to spawn a UFO.
        /// </summary>
        private const float MaximumSpawnTime = 20;

        /// <summary>
        /// The horizontal speed of the UFO.
        /// </summary>
        private const int HorizontalSpeed = 100;

        /// <summary>
        /// The vertical padding, in pixels, applied to the spawn position of the UFO.
        /// </summary>
        private const int VerticalSpawnPadding = 10;

        /// <summary>
        /// The amount of time, in seconds, that the score remains hidden during a single flash.
        /// </summary>
        private const float ScoreFlashHiddenTime = 0.05f;

        /// <summary>
        /// The amount of time, in seconds, between flashes.
        /// </summary>
        private const float ScoreFlashBreakTime = 0.10f;

        /// <summary>
        /// The amount of times the score flashes.
        /// </summary>
        private const int ScoreFlashFrequency = 3;

        private readonly int[] scores;
        private readonly Texture2D ufoTexture;
        private readonly SpriteFont spaceInvadersFont;

        private float timeToSpawn;

        private RectangleF BoundingRectangle;
        private int movementDirection;
        private bool isUfoActive;

        private bool flashScore;
        private bool isScoreVisible;
        private float flashScoreTimer;
        private int scoreFlashCounter;
        private int points;

        /// <summary>
        /// Initializes a new <see cref="UfoController"/>.
        /// </summary>
        public UfoController()
        {
            JsonObject jsonObject = MainGame.Context.Content.Load<JsonObject>("UfoScores");
            scores = JsonConvert.DeserializeObject<int[]>(jsonObject.JsonSource);
            spaceInvadersFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");

            timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);
            ufoTexture = MainGame.Context.MainTextureAtlas["ufo"];
            BoundingRectangle = new RectangleF(0, 0, ufoTexture.Width * MainGame.ResolutionScale,
                ufoTexture.Height * MainGame.ResolutionScale);

            flashScore = false;
        }

        /// <summary>
        /// Update the <see cref="UfoController"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay).IsFrozen) return;

            float ufoWidth = ufoTexture.Width * MainGame.ResolutionScale;
            if (flashScore)
            {
                UpdateScoreFlash(deltaTime);
                return;
            }

            if (isUfoActive)
            {
                BoundingRectangle.X += HorizontalSpeed * movementDirection * deltaTime;

                // Check if the UFO has gone out of bounds
                if (movementDirection == 1 && BoundingRectangle.X - ufoWidth * MainGame.ResolutionScale > MainGame.GameScreenWidth ||
                    movementDirection == -1 && BoundingRectangle.X < -ufoWidth)
                {
                    Destroy(false);
                }
            }
            else
            {
                timeToSpawn -= deltaTime;
                if (!(timeToSpawn <= 0)) return;

                // A 50% chance to spawn on the left side.
                bool left = Random.Value() > 0.5f;
                movementDirection = left ? 1 : -1;
                isUfoActive = true;

                float positionY = GameplayScreen.TopVerticalBoundary + ufoTexture.Height * MainGame.ResolutionScale + VerticalSpawnPadding;
                float positionX = left ? -ufoWidth : MainGame.GameScreenWidth + ufoWidth;
                BoundingRectangle.Position = new Vector2(positionX, positionY);
                timeToSpawn = Random.Range(MinimumSpawnTime, MaximumSpawnTime);
            }
        }

        /// <summary>
        /// Update the flashing effect.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateScoreFlash(float deltaTime)
        {
            flashScoreTimer -= deltaTime;
            if (flashScoreTimer <= 0)
            {
                flashScoreTimer = isScoreVisible ? ScoreFlashHiddenTime : ScoreFlashBreakTime;
                isScoreVisible = !isScoreVisible;
                if (isScoreVisible)
                {
                    scoreFlashCounter += 1;
                }
            }

            if (scoreFlashCounter >= ScoreFlashFrequency)
            {
                flashScore = false;
            }
        }

        /// <summary>
        /// Render the UFO and flash effect.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (flashScore && isScoreVisible)
            {
                spriteBatch.DrawString(spaceInvadersFont, points.ToString(), BoundingRectangle.Position, Color.White);
            }

            if (!isUfoActive) return;
            spriteBatch.Draw(ufoTexture, BoundingRectangle.Position, null, Color.Red, 0, Vector2.Zero, 
                MainGame.ResolutionScale, SpriteEffects.None, 0.5f);
        }

        /// <summary>
        /// Destroy the active UFO.
        /// </summary>
        /// <param name="playerHit">
        /// A boolean indicating whether the player hit the UFO.
        /// If this value is false, no score flash animation occurs.
        /// </param>
        public void Destroy(bool playerHit = true)
        {
            isUfoActive = false;
            if (!playerHit) return;

            points = scores.Choose();
            flashScore = true;
            isScoreVisible = true;
            scoreFlashCounter = 0;
            flashScoreTimer = ScoreFlashBreakTime;

            MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay).Player.Score += points;
        }

        /// <summary>
        /// Determines whether this <see cref="UfoController"/> is intersecting the specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="rectangle">The <see cref="RectangleF"/>.</param>
        /// <returns>A boolean value indicating whether an intersection occured.</returns>
        public bool Intersects(RectangleF rectangle) => isUfoActive && BoundingRectangle.Intersects(rectangle);
    }
}
