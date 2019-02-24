/*
 * Author: Shon Verch
 * File Name: EnemyType.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/24/2019
 * Description: A data-class describing a type of enemy.
 */

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Logging;

namespace SpaceInvaders
{
    /// <summary>
    /// A data-class describing a type of <see cref="Enemy"/>.
    /// </summary>
    public struct EnemyType
    {
        /// <summary>
        /// The null <see cref="EnemyType"/>.
        /// </summary>
        public static EnemyType None { get; }

        /// <summary>
        /// A mapping of names to <see cref="EnemyType"/> instances.
        /// <remarks>
        /// Used for O(1) queries.
        /// </remarks>
        /// </summary>
        private static readonly Dictionary<string, EnemyType> enemyTypeNameMap;

        /// <summary>
        /// The name of this <see cref="EnemyType"/>.
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }

        /// <summary>
        /// The amount of points rewarded to the <see cref="Player"/> when an <see cref="Enemy"/> of this <see cref="EnemyType"/> is destroyed.
        /// </summary>
        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; }

        static EnemyType()
        {
            enemyTypeNameMap = new Dictionary<string, EnemyType>();
            None = new EnemyType("None", 0);
            Add(None);
        }

        /// <summary>
        /// Initialize a new <see cref="EnemyType"/>.
        /// </summary>
        /// <param name="name">The name of this <see cref="EnemyType"/>.</param>
        /// <param name="points">The amount of points rewarded to the <see cref="Player"/> when an <see cref="Enemy"/> of this <see cref="EnemyType"/> is destroyed.</param>
        public EnemyType(string name, int points)
        {
            Name = name;
            Points = points;
        }

        /// <summary>
        /// Loads the enemy types from JSON.
        /// </summary>
        public static void Load(ContentManager contentManager)
        {
            string json = contentManager.Load<JsonObject>("EnemyTypes").JsonSource;
            EnemyType[] enemyTypes = JsonConvert.DeserializeObject<EnemyType[]>(json);
            foreach (EnemyType enemyType in enemyTypes)
            {
                Add(enemyType);
            }
        }

        public static EnemyType Parse(string name)
        {
            if (enemyTypeNameMap.ContainsKey(name)) return enemyTypeNameMap[name];
            Logger.LogFunctionEntry(string.Empty, $"Enemy Type \"{name}\" could not be found!", LoggerVerbosity.Warning);
            return None;
        }

        public static void Add(EnemyType enemyType)
        {
            if (enemyTypeNameMap.ContainsKey(enemyType.Name))
            {
                Logger.LogFunctionEntry(string.Empty, $"Enemy Type \"{enemyType.Name}\" already exists!", LoggerVerbosity.Warning);
                return;
            }

            enemyTypeNameMap.Add(enemyType.Name, enemyType);
        }

        /// <summary>
        /// Gets all the loaded <see cref="EnemyType"/> instances.
        /// </summary>
        /// <param name="includeNone">Indicates whether the <see cref="None"/> should be included in the result.</param>
        /// <returns>An array of <see cref="EnemyType"/> instances.</returns>
        public static EnemyType[] All(bool includeNone = false) => 
            enemyTypeNameMap.Values.Where(type => includeNone || type != None).ToArray();

        /// <summary>
        /// Determines whether the two <see cref="EnemyType"/> values are the same.
        /// </summary>
        /// <param name="a">The first <see cref="EnemyType"/>.</param>
        /// <param name="b">The second <see cref="EnemyType"/>.</param>
        /// <returns>
        /// A boolean indicating whether the two values are the same.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EnemyType a, EnemyType b) => a.Equals(b);

        /// <summary>
        /// Determines whether the two <see cref="EnemyType"/> values are not the same.
        /// </summary>
        /// <param name="a">The first <see cref="EnemyType"/>.</param>
        /// <param name="b">The second <see cref="EnemyType"/>.</param>
        /// <returns>
        /// A boolean indicating whether the two values are not the same.
        /// Value <c>true</c> if they are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(EnemyType a, EnemyType b) => !a.Equals(b);

        /// <summary>
        /// Determines whether this <see cref="EnemyType"/> is equal to the specified <see cref="EnemyType"/>.
        /// </summary>
        /// <param name="other">The object.</param>
        /// <returns>
        /// A boolean value indicating whether this <see cref="EnemyType"/> is equal to the specified <see cref="EnemyType"/>.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EnemyType other) => string.Equals(Name, other.Name) && Points == other.Points;

        /// <summary>
        /// Determines whether this <see cref="EnemyType"/> is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A boolean value indicating whether this <see cref="EnemyType"/> is equal to the specified object.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is EnemyType other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code of this <see cref="EnemyType"/>.
        /// </summary>
        /// <returns>A hash code of this <see cref="EnemyType"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Points;
            }
        }

        /// <summary>
        /// Converts this <see cref="EnemyType"/> to a string representation.
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        /// Converts a <see cref="EnemyType"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="type">The <see cref="EnemyType"/> to convert.</param>
        public static explicit operator string(EnemyType type) => type.ToString();
    }
}
