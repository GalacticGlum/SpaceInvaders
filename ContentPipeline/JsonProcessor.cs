/*
 * Author: Shon Verch
 * File Name: JsonProcessor.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/19
 * Modified Date: 02/05/19
 * Description: The content processor for JSON files.
 */

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SpaceInvaders.ContentPipeline
{
    /// <summary>
    /// The content processor for JSON files.
    /// </summary>
    [ContentProcessor(DisplayName = "JSON Processor")]
    public class JsonProcessor : ContentProcessor<string, JsonFileData>
    {
        /// <summary>
        /// Processes a JSON data from a string source.
        /// </summary>
        /// <param name="input">The string containing the JSON data.</param>
        /// <param name="context">The processor context.</param>
        /// <returns>A <see cref="JsonFileData"/> instance, the result of the data processing.</returns>
        public override JsonFileData Process(string input, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing JSON file");
                return new JsonFileData(input);

            }
            catch (Exception error)
            {
                context.Logger.LogMessage($"Error {error}");
                throw;
            }
        }
    }
}
