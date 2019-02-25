/*
 * Author: Shon Verch
 * File Name: ProjectileController.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/09/2019
 * Modified Date: 02/23/2019
 * Description: The top-level manager for all projectile instances.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Engine;
using Random = SpaceInvaders.Engine.Random;

namespace SpaceInvaders
{
    /// <summary>
    /// The top-level manager for all projectile instances.
    /// </summary>
    public class ProjectileController
    {
        private readonly Dictionary<ProjectileType, List<Projectile>> projectilePrototypes;

        /// <summary>
        /// A list containing all the <see cref="Projectile"/> objects to destroy
        /// at the end of the frame.
        /// <remarks>
        /// This is because if the player fires a projectile while we are traversing the
        /// projectile collection (i.e. during draw or update), a CollectionWasModified
        /// exception would be thrown since the collection was modified during traversal.
        /// In order to get around this, we can modify the list AFTER the traversal.
        /// </remarks>
        /// </summary>
        private readonly List<Projectile> destroyList;

        /// <summary>
        /// The current active projectiles shot by enemies.
        /// </summary>
        private readonly Dictionary<ProjectileType, List<Projectile>> activateProjectiles;

        private readonly SoundEffectInstance shootSoundEffectInstance;

        /// <summary>
        /// Initializes a new <see cref="ProjectileController"/>.
        /// </summary>
        public ProjectileController()
        {
            activateProjectiles = new Dictionary<ProjectileType, List<Projectile>>();
            projectilePrototypes = new Dictionary<ProjectileType, List<Projectile>>();
            destroyList = new List<Projectile>();

            LoadProjectilePrototypes();
            shootSoundEffectInstance = MainGame.Context.Content.Load<SoundEffect>("Audio/shoot").CreateInstance();
            shootSoundEffectInstance.Volume = MainGame.Volume;
        }

        /// <summary>
        /// Load all the projectile prototypes.
        /// </summary>
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
                    // create the list later. This means that we don't need to check whether
                    // the collection contains the type when we want to modify it.
                    activateProjectiles[projectile.Type] = new List<Projectile>();
                }

                projectilePrototypes[projectile.Type].Add(projectile);
            }
        }

        /// <summary>
        /// Spawn a projectile from the specified protoype, at the specified position.
        /// </summary>
        /// <param name="postion">The position of the new projectile.</param>
        /// <param name="prototype">The protoype to clone.</param>
        private void CreateProjectile(Vector2 postion, Projectile prototype)
        {
            activateProjectiles[prototype.Type].Add(new Projectile(prototype, postion));
        }

        /// <summary>
        /// Fires a player projectile
        /// if one does not exist yet.
        /// </summary>
        public void CreatePlayerProjectile()
        {
            GameplayScreen gameplayScreen = MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay);

            // There already exists a player projectile in the world.
            // A player can only fire a projectile after the previous one
            // has been destroyed.
            if (activateProjectiles.ContainsKey(ProjectileType.Player) && activateProjectiles[ProjectileType.Player].Count > 0) return;

            // The projectile should spawn at the top-centre of the player.
            // The position of the player is the top-left so we just need to
            // horizontally offset it by half of the player width.
            Vector2 offset = new Vector2(gameplayScreen.Player.Texture.Width * MainGame.ResolutionScale * 0.5f, 0);
            Vector2 position = gameplayScreen.Player.Position + offset;

            // Get a random player projectile
            CreateProjectile(position, GetRandomProjectilePrototype(ProjectileType.Player));
            shootSoundEffectInstance.Play();
        }

        /// <summary>
        /// Fires an enemy projectile.
        /// </summary>
        /// <param name="enemy"></param>
        public void CreateEnemyProjectile(Enemy enemy)
        {
            RectangleF enemyRectangle = MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay).EnemyGroup.GetEnemyWorldRectangle(enemy);
            Vector2 position = enemyRectangle.Position + new Vector2(enemyRectangle.Width / 2, 0);

            CreateProjectile(position, GetRandomProjectilePrototype(ProjectileType.Enemy));
            shootSoundEffectInstance.Play();
        }

        /// <summary>
        /// Removes the specified projectile from the world.
        /// </summary>
        /// <param name="projectile">The projectile to remove.</param>
        public void Remove(Projectile projectile)
        {
            if (!activateProjectiles.ContainsKey(projectile.Type)) return;
            destroyList.Add(projectile);
        }

        /// <summary>
        /// Update all the projectiles.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay).IsFrozen) return;
            ApplyOperationOnProjectiles(projectile => projectile.Update(deltaTime));
            CollectProjectiles();
        }

        /// <summary>
        /// Collect the projectiles that need to be destroyed.
        /// </summary>
        private void CollectProjectiles()
        {
            foreach (Projectile projectile in destroyList)
            {
                activateProjectiles[projectile.Type].Remove(projectile);
            }
        }

        /// <summary>
        /// Render all the projectiles.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            ApplyOperationOnProjectiles(projectile => projectile.Draw(spriteBatch));
        }

        /// <summary>
        /// Applis an operation on each projectile.
        /// </summary>
        /// <param name="operation">The <see cref="Action{T}"/> to apply on each projectile.</param>
        private void ApplyOperationOnProjectiles(Action<Projectile> operation)
        {
            foreach (List<Projectile> projectiles in activateProjectiles.Values)
            {
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
