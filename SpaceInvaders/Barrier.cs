/*
 * Author: Shon Verch
 * File Name: Barrier.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/06/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Helpers;

namespace SpaceInvaders
{
    public class Barrier
    {
        /// <summary>
        /// The amount of pixels to spawn above the player.
        /// </summary>
        private const int VerticalSpawnOffset = 10;

        private readonly Vector2 position;

        private readonly int tileWidth;
        private readonly int tileHeight;

        private readonly BarrierTile[,] tiles;
        
        public Barrier(int spawnIndex)
        {
            string jsonSource = MainGame.Context.Content.Load<JsonObject>("BarrierLayout").JsonSource;
            string[,] layout = JsonConvert.DeserializeObject<string[,]>(jsonSource);

            tileWidth = layout.GetLength(1);
            tileHeight = layout.GetLength(0);

            tiles = new BarrierTile[tileWidth, tileHeight];
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    tiles[x, y] = new BarrierTile(new Vector2(x, y), layout[y, x]);
                }
            }

            // We need the height of a barrier tile so that we can
            // offset that spawn y-coordinate properly. Since all the tiles
            // have the same height, we can load in an arbitrary barrier tile texture.
            // For simplicity sake, we use the first element of the tiles grid. 
            Texture2D barrierTileTexture = MainGame.Context.MainTextureAtlas[tiles[0, 0].TextureName];

            float deltaX = (MainGame.HorizontalBoundaryStart.X + MainGame.HorizontalBoundaryEnd.X) / (BarrierGroup.SpawnBarrierCount + 1);
            float xCorrection = tileWidth * barrierTileTexture.Width * MainGame.SpriteScaleFactor * 0.5f;
            float spawnX = MainGame.HorizontalBoundaryStart.X + deltaX * (spawnIndex + 1) - xCorrection;

            float yCorrection = tileHeight * barrierTileTexture.Height * MainGame.SpriteScaleFactor;
            float spawnY =  MainGame.Context.Player.Position.Y - VerticalSpawnOffset * MainGame.SpriteScaleFactor - yCorrection;
            position = new Vector2(spawnX, spawnY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    BarrierTile tile = tiles[x, y];

                    Texture2D texture = MainGame.Context.MainTextureAtlas[tile.TextureName];
                    float offsetX = x * texture.Width * MainGame.SpriteScaleFactor;
                    float offsetY = y * texture.Height * MainGame.SpriteScaleFactor;

                    spriteBatch.Draw(texture, position + new Vector2(offsetX, offsetY), null, ColourHelpers.PureGreen, 0,
                        Vector2.Zero, MainGame.SpriteScaleFactor, SpriteEffects.None, 1);
                }
            }
        }
    }
}
