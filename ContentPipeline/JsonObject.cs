/*
 * Author: Shon Verch
 * File Name: JsonObject.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: A data object containing the result of the JsonReader.
 *              This type is used in Content.Load calls.
 */

namespace SpaceInvaders.ContentPipeline
{
    /// <summary>
    /// A data object containing the result of the <see cref="JsonReader"/>.
    /// This type is used in <c>Content.Load</c> calls.
    /// </summary>
    public class JsonObject
    {
        public string JsonSource { get; }
        public dynamic Data { get; }

        public JsonObject(string jsonSource, dynamic data)
        {
            JsonSource = jsonSource;
            Data = data;
        }
    }
}