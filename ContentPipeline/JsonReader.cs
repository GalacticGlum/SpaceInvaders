/*
 * Author: Shon Verch
 * File Name: JsonReader.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace SpaceInvaders.ContentPipeline
{
    public class JsonReader : ContentTypeReader<JsonObject>
    {
        protected override JsonObject Read(ContentReader input, JsonObject existingInstance)
        {
            string json = input.ReadString();
            return new JsonObject(json, JsonConvert.DeserializeObject<dynamic>(json));
        }
    }
}
