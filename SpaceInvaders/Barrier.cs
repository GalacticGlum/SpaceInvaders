/*
 * Author: Shon Verch
 * File Name: Barrier.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/14/019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class Barrier
    {
        /// <summary>
        /// The amount of pixels to spawn above the player.
        /// </summary>
        private const int VerticalSpawnOffset = 10;

        public RectangleF Rectangle { get; }

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
            float totalWidth = tileWidth * barrierTileTexture.Width * MainGame.ResolutionScale;
            float totalHeight = tileHeight * barrierTileTexture.Height * MainGame.ResolutionScale;

            float deltaX = (MainGame.HorizontalBoundaryStart.X + MainGame.HorizontalBoundaryEnd.X) / (BarrierGroup.SpawnBarrierCount + 1);
            float xCorrection = totalWidth * 0.5f;
            float spawnX = MainGame.HorizontalBoundaryStart.X + deltaX * (spawnIndex + 1) - xCorrection;
            float spawnY =  MainGame.Context.Player.Position.Y - VerticalSpawnOffset * MainGame.ResolutionScale - totalHeight;

            Rectangle = new RectangleF(spawnX, spawnY, totalWidth, totalHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    tiles[x, y].Draw(Rectangle.Position, spriteBatch);
                }
            }
        }

        public bool Intersects(RectangleF rectangle, out BarrierTile intersectionTile)
        {
            intersectionTile = null;
            // As a preliminary check, let's see if this rectangle
            // intersects the bounding box at all. Then, we can check
            // the individual tiles.
            if (!rectangle.Intersects(Rectangle)) return false;

            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    BarrierTile tile = tiles[x, y];
                    if (!tile.Active || !rectangle.Intersects(tile.GetWorldRectangle(Rectangle.Position))) continue;

                    intersectionTile = tile;
                    return true;
                }
            }

            return false;
        }
    }
}
