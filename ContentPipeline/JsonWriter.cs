using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace SpaceInvaders.ContentPipeline
{
    [ContentTypeWriter]
    public class JsonWriter : ContentTypeWriter<JsonFileData>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) =>
            typeof(JsonReader).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, JsonFileData data)
        {
            output.Write(data.JsonData);
        }
    }
}
