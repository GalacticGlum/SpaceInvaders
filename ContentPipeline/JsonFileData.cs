/*
 * Author: Shon Verch
 * File Name: JsonFileData.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: A wrapper class that contains the result of the JsonProcessor.
 */

namespace SpaceInvaders.ContentPipeline
{
    /// <summary>
    /// <para>A wrapper class that contains the result of the <see cref="JsonProcessor"/>.</para>
    /// Though this class is only a wrapper, it is necessary because the MonoGame
    /// content pipeline uses the output type of the <see cref="JsonProcessor"/> as
    /// a unique key.
    /// </summary>
    public struct JsonFileData
    {
        /// <summary>
        /// The JSON source string.
        /// </summary>
        public string JsonData { get; }

        /// <summary>
        /// Initializes a new <see cref="JsonFileData"/>.
        /// </summary>
        /// <param name="jsonData">The JSON source string.</param>
        public JsonFileData(string jsonData)
        {
            JsonData = jsonData;
        }
    }
}