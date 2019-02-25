/*
 * Author: Shon Verch
 * File Name: JsonReader.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: The content reader for JSON objects.
 */

using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace SpaceInvaders.ContentPipeline
{
    /// <summary>
    /// The content reader for <see cref="JsonObject"/>.
    /// </summary>
    public class JsonReader : ContentTypeReader<JsonObject>
    {
        /// <summary>
        /// Reads a <see cref="JsonObject"/>.
        /// </summary>
        protected override JsonObject Read(ContentReader input, JsonObject existingInstance)
        {
            string json = input.ReadString();
            return new JsonObject(json);
        }
    }
}
