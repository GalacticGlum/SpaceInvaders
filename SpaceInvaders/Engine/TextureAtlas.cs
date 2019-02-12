/*
 * Author: Shon Verch
 * File Name: TextureAtlas.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/12/2019
 * Description: DESCRIPTION
 */

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Logging;

namespace SpaceInvaders.Engine
{
    public struct TextureAtlasEntry
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("rectangle", Required = Required.Always)]
        public Rectangle Rectangle { get; set; }
    }

    public class TextureAtlas
    {
        public Texture2D this[string name] => Get(name);

        private readonly Dictionary<string, Texture2D> textureAtlasEntries;

        public TextureAtlas(string atlasContentFilePath, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            textureAtlasEntries = new Dictionary<string, Texture2D>();
            Texture2D textureAtlas = contentManager.Load<Texture2D>(atlasContentFilePath);

            string json = contentManager.Load<JsonObject>(Path.ChangeExtension(atlasContentFilePath, "meta")).JsonSource;
            TextureAtlasEntry[] metaDataEntries = JsonConvert.DeserializeObject<TextureAtlasEntry[]>(json);
            foreach (TextureAtlasEntry entry in metaDataEntries)
            {
                textureAtlasEntries[entry.Name] = TextureHelpers.GetCroppedTexture(textureAtlas, entry.Rectangle, graphicsDevice);
            }

            // Since we have load all the textures from our atlas, there is no
            // reason to keep the texture atlas data allocated; hence, we can unload it.
            contentManager.Unload();
        }

        public Texture2D Get(string name)
        {
            if (textureAtlasEntries.ContainsKey(name)) return textureAtlasEntries[name];

            Logger.LogFunctionEntry(string.Empty, $"Could not find texture with name \"{name}\".", LoggerVerbosity.Warning);
            return null;
        }
    }
}
