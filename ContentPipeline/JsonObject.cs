/*
 * Author: Shon Verch
 * File Name: JsonObject.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

namespace SpaceInvaders.ContentPipeline
{
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