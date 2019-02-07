/*
 * Author: Shon Verch
 * File Name: Barrier.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/06/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;

namespace SpaceInvaders
{
    public class Barrier
    {
        private readonly Vector2 position;
        private readonly BarrierTile[,] tiles;

        public Barrier(Vector2 position, ContentManager contentManager)
        {
            this.position = position;

            string jsonSource = contentManager.Load<JsonObject>("BarrierLayout").JsonSource;
            string[,] layout = JsonConvert.DeserializeObject<string[,]>(jsonSource);

            int layoutWidth = layout.GetLength(1);
            int layoutHeight = layout.GetLength(0);

            tiles = new BarrierTile[layoutWidth, layoutHeight];
            for (int y = 0; y < layoutHeight; y++)
            {
                for (int x = 0; x < layoutWidth; x++)
                {
                    tiles[x, y] = new BarrierTile(new Vector2(x, y), layout[y, x]);
                }
            }
        }
    }
}
