/*
 * Author: Shon Verch
 * File Name: MainGame.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Helpers;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        private const int GameScreenWidth = 628;
        private const int GameScreenHeight = 580;
        private const int HorizontalBoundarySize = 5;

        /// <summary>
        /// The y-coordinate of the horizontal boundary line.
        /// The line is placed 1/8 above the bottom of the screen;
        /// or 7/8 (0.875) below the top of the screen.
        /// </summary>
        private const float HorizontalBoundaryY = GameScreenHeight * 0.875f;

        /// <summary>
        /// The starting point of the horizontal boundary line.
        /// </summary>
        private static readonly Vector2 HorizontalBoundaryStart = new Vector2(HorizontalBoundarySize, HorizontalBoundaryY);

        /// <summary>
        /// The ending point of the horizontal boundary line.
        /// </summary>
        private static readonly Vector2 HorizontalBoundaryEnd = new Vector2(GameScreenWidth - HorizontalBoundarySize, HorizontalBoundaryY);

        private TextureAtlas mainTextureAtlas;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainTextureAtlas = new TextureAtlas("MainAtlas", GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawLine(HorizontalBoundaryStart, HorizontalBoundaryEnd, ColourHelpers.PureGreen, 2);
            spriteBatch.End();
        }
    }
}
