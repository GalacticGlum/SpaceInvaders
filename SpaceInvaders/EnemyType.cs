/*
 * Author: Shon Verch
 * File Name: EnemyType.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/07/2019
 * Description: DESCRIPTION
 */

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Logging;

namespace SpaceInvaders
{
    public struct EnemyType
    {
        public static EnemyType None { get; }
        private static readonly Dictionary<string, EnemyType> enemyTypeNameMap;

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }

        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; }

        static EnemyType()
        {
            enemyTypeNameMap = new Dictionary<string, EnemyType>();
            None = new EnemyType("None", 0);
            Add(None);
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

        public EnemyType(string name, int points)
        {
            Name = name;
            Points = points;
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

        public static EnemyType[] All(bool includeNone = false) => 
            enemyTypeNameMap.Values.Where(type => includeNone || type != None).ToArray();

        public static bool operator ==(EnemyType a, EnemyType b) => a.Equals(b);
        public static bool operator !=(EnemyType a, EnemyType b) => !a.Equals(b);

        public bool Equals(EnemyType other) => string.Equals(Name, other.Name) && Points == other.Points;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is EnemyType other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Points;
            }
        }

        public override string ToString() => Name;
        public static explicit operator string(EnemyType type) => type.ToString();
    }
}
