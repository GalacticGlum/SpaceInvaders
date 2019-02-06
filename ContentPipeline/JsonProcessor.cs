using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SpaceInvaders.ContentPipeline
{
    [ContentProcessor(DisplayName = "JSON Processor")]
    public class JsonProcessor : ContentProcessor<string, JsonFileData>
    {
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
