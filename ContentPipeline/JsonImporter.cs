/*
 * Author: Shon Verch
 * File Name: JsonImporter.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: The content importer for JSON files.
 */

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SpaceInvaders.ContentPipeline
{
    /// <summary>
    /// The content importer for JSON files.
    /// </summary>
    [ContentImporter(".json", DefaultProcessor = "JsonProcessor", DisplayName = "JSON Importer")]
    public class JsonImporter : ContentImporter<string>
    {
        /// <summary>
        /// Imports a JSON file into the content pipeline.
        /// </summary>
        /// <param name="filename">The name of the content file to import.</param>
        /// <param name="context">The content pipeline context.</param>
        /// <returns>The JSON source string.</returns>
        public override string Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing JSON file: {filename}");
            return File.ReadAllText(filename);
        }
    }
}
