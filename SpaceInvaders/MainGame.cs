/*
 * Author: Shon Verch
 * File Name: MainGame.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/11/2019
 * Description: The core engine instance of the game that spawns all other entities
 *              and simulates logic and rendering.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;
using SpaceInvaders.Logging;

namespace SpaceInvaders
{
    /// <summary>
    /// The core engine instance of the game that spawns
    /// all other entities and simulates logic and rendering.
    /// </summary>
    public class MainGame : Game
    {
        public const int GameScreenWidth = 628;
        public const int GameScreenHeight = 580;
        public const int HorizontalBoundarySize = 5;

        /// <summary>
        /// The scale factor of all game sprites.
        /// </summary>
        public const float ResolutionScale = 2.5f;

        /// <summary>
        /// The y-coordinate of the horizontal boundary line.
        /// The line is placed 1/8 above the bottom of the screen;
        /// or 7/8 (0.875) below the top of the screen.
        /// </summary>
        public const float HorizontalBoundaryY = GameScreenHeight * 0.875f;

        /// <summary>
        /// The vertical padding, in pixels, from the top of the screen
        /// that is allocated for UI elements.
        /// This value is 10% of the screen height.
        /// </summary>
        public const float TopVerticalBoundary = GameScreenHeight * 0.1f;

        /// <summary>
        /// The starting point of the horizontal boundary line.
        /// </summary>
        public static readonly Vector2 HorizontalBoundaryStart = new Vector2(HorizontalBoundarySize, HorizontalBoundaryY);

        /// <summary>
        /// The ending point of the horizontal boundary line.
        /// </summary>
        public static readonly Vector2 HorizontalBoundaryEnd = new Vector2(GameScreenWidth - HorizontalBoundarySize, HorizontalBoundaryY);


        /// <summary>
        /// The current instance of this <see cref="MainGame"/>.
        /// </summary>
        public static MainGame Context { get; private set; }

        /// <summary>
        /// The main texture atlas containing the player, enemy, and combat sprites.
        /// </summary>
        public TextureAtlas MainTextureAtlas { get; private set; }

        public Player Player { get; private set; }
        public EnemyGroup EnemyGroup { get; private set; }
        public BarrierGroup BarrierGroup { get; private set; }
        public ProjectileController ProjectileController { get; private set; }

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Context = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = GameScreenWidth;
            graphics.PreferredBackBufferHeight = GameScreenHeight;
            graphics.ApplyChanges();

            Logger.Destination = LoggerDestination.File;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MainTextureAtlas = new TextureAtlas("MainAtlas", GraphicsDevice, Content);

            // Load all the enemy types
            EnemyType.Load(Content);

            Player = new Player();
            EnemyGroup = new EnemyGroup();
            BarrierGroup = new BarrierGroup();
            ProjectileController = new ProjectileController();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            // We NEED to update input before we execute game logic
            // so that the gameplay does not lag by a frame (due to not synchronized input).
            Input.Update();

            Player.Update(deltaTime);
            EnemyGroup.Update(deltaTime);
            ProjectileController.Update(deltaTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.DrawLine(HorizontalBoundaryStart, HorizontalBoundaryEnd, ColourHelpers.PureGreen, 2);

            Player.Draw(spriteBatch);
            EnemyGroup.Draw(spriteBatch);
            BarrierGroup.Draw(spriteBatch);
            ProjectileController.Draw(spriteBatch);

            spriteBatch.End();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Logger.FlushMessageBuffer();
        }
    }
}
