/*
 * Author: Shon Verch
 * File Name: TextureAtlas.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;

namespace SpaceInvaders.Helpers
{
    public class TextureAtlasEntry
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("rectangle", Required = Required.Always)]
        public Rectangle Rectangle { get; set; }

        [JsonIgnore]
        public Texture2D Texture { get; set; } = null;
    }

    public class TextureAtlas
    {
        public Texture2D this[string name] => Get(name);

        private readonly Texture2D textureAtlas;
        private readonly Dictionary<string, TextureAtlasEntry> textureAtlasEntries;

        private readonly GraphicsDevice graphicsDevice;

        public TextureAtlas(string atlasContentFilePath, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            this.graphicsDevice = graphicsDevice;

            textureAtlas = contentManager.Load<Texture2D>(atlasContentFilePath);
            textureAtlasEntries = new Dictionary<string, TextureAtlasEntry>();

            LoadMetaData(atlasContentFilePath, contentManager);
        }

        public TextureAtlas(Texture2D textureAtlas, string atlasMetaDataFilePath, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            this.graphicsDevice = graphicsDevice;
            this.textureAtlas = textureAtlas;

            textureAtlasEntries = new Dictionary<string, TextureAtlasEntry>();
            LoadMetaData(atlasMetaDataFilePath, contentManager);
        }

        private void LoadMetaData(string atlasMetaDataFilePath, ContentManager contentManager)
        {
            string json = contentManager.Load<JsonObject>(Path.ChangeExtension(atlasMetaDataFilePath, "meta")).JsonSource;
            TextureAtlasEntry[] metaDataEntries = JsonConvert.DeserializeObject<TextureAtlasEntry[]>(json);
            foreach (TextureAtlasEntry entry in metaDataEntries)
            {
                textureAtlasEntries[entry.Name] = entry;
            }
        }

        public Texture2D Get(string name, bool forceReload = false)
        {
            if (!textureAtlasEntries.ContainsKey(name)) return null;
            TextureAtlasEntry entry = textureAtlasEntries[name];
            if (!forceReload && entry.Texture != null)
            {
                return entry.Texture;
            }

            Texture2D texture = TextureHelpers.GetCroppedTexture(textureAtlas, entry.Rectangle, graphicsDevice);
            entry.Texture = texture;

            return texture;
        }
    }
}
