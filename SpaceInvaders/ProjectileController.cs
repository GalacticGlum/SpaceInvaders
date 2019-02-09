/*
 * Author: Shon Verch
 * File Name: ProjectileController.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/09/2019
 * Modified Date: 02/09/2019
 * Description: DESCRIPTION
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using Random = SpaceInvaders.Helpers.Random;

namespace SpaceInvaders
{
    public class ProjectileController
    {
        private readonly Dictionary<ProjectileType, List<Projectile>> projectilePrototypes;

        /// <summary>
        /// The current active projectiles shot by enemies.
        /// </summary>
        private readonly Dictionary<ProjectileType, HashSet<Projectile>> activateProjectiles;

        public ProjectileController()
        {
            activateProjectiles = new Dictionary<ProjectileType, HashSet<Projectile>>();
            projectilePrototypes = new Dictionary<ProjectileType, List<Projectile>>();
            LoadProjectilePrototypes();
        }

        private void LoadProjectilePrototypes()
        {
            string jsonSource = MainGame.Context.Content.Load<JsonObject>("BulletTypes").JsonSource;
            Projectile[] projectiles = JsonConvert.DeserializeObject<Projectile[]>(jsonSource);
            foreach (Projectile projectile in projectiles)
            {
                if (!projectilePrototypes.ContainsKey(projectile.Type))
                {
                    projectilePrototypes[projectile.Type] = new List<Projectile>();

                    // "Warm-up" our active projectiles dictionary so that we don't have to
                    // create the hashset later. The benefits are twofold:
                    //      a) we don't need to check whether the collection contains the type
                    //         when we want to modify it.
                    //      b) We don't need to worry about copying the collection when we apply
                    //         an operation to it (to avoid a CollectionWasModified exception) since
                    //         the dictionary will remain unchanged. Hence, we only need to copy the
                    //         underlying hash set containing the projectiles.
                    activateProjectiles[projectile.Type] = new HashSet<Projectile>();
                }

                projectilePrototypes[projectile.Type].Add(projectile);
            }
        }

        /// <summary>
        /// Fires a player projectile
        /// if one does not exist yet.
        /// </summary>
        public void CreatePlayerProjectile()
        {
            // There already exists a player projectile in the world.
            // A player can only fire a projectile after the previous one
            // has been destroyed.
            if (activateProjectiles.ContainsKey(ProjectileType.Player) && activateProjectiles[ProjectileType.Player].Count > 0) return;

            // The projectile should spawn at the top-centre of the player.
            // The position of the player is the top-left so we just need to
            // horizontally offset it by half of the player width.
            Vector2 offset = new Vector2(MainGame.Context.Player.Texture.Width * MainGame.SpriteScaleFactor * 0.5f, 0);
            Vector2 position = MainGame.Context.Player.Position + offset;

            // Get a random player projectile
            Projectile prototype = GetRandomProjectilePrototype(ProjectileType.Player);
            activateProjectiles[ProjectileType.Player].Add(new Projectile(prototype, position));
        }

        /// <summary>
        /// Removes the specified projectile from the world.
        /// </summary>
        /// <param name="projectile">The projectile to remove.</param>
        public void Remove(Projectile projectile)
        {
            if (!activateProjectiles.ContainsKey(projectile.Type)) return;
            activateProjectiles[projectile.Type].Remove(projectile);
        }

        public void Update(float deltaTime)
        {
            ApplyOperationOnProjectiles(projectile => projectile.Update(deltaTime));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ApplyOperationOnProjectiles(projectile => projectile.Draw(spriteBatch));
        }

        private void ApplyOperationOnProjectiles(Action<Projectile> operation)
        {
            foreach (HashSet<Projectile> projectileSet in activateProjectiles.Values)
            {
                if (projectileSet.Count == 0) continue;

                // We need to copy the value collection into memory to avoid modification errors.
                // In particular, if the player fires a projectile while we are traversing this
                // collection, a CollectionWasModified exception will be thrown since the collection
                // was modified during traversal.
                Projectile[] projectiles = new Projectile[projectileSet.Count];
                projectileSet.CopyTo(projectiles, 0);

                foreach (Projectile projectile in projectiles)
                {
                    operation(projectile);
                }
            }
        }

        /// <summary>
        /// Retrieves a random projectile prototype with the specified type.
        /// </summary>
        /// <param name="type">The type of the projectile prototype.</param>
        private Projectile GetRandomProjectilePrototype(ProjectileType type) => 
            projectilePrototypes[type][Random.Range(projectilePrototypes[type].Count)];
    }
}
