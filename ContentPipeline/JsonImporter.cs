using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SpaceInvaders.ContentPipeline
{
    [ContentImporter(".json", DefaultProcessor = "JsonProcessor", DisplayName = "JSON Importer")]
    public class JsonImporter : ContentImporter<string>
    {
        public override string Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing JSON file: {filename}");
            return File.ReadAllText(filename);
        }
    }
}
