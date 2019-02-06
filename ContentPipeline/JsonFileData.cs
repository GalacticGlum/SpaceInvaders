namespace SpaceInvaders.ContentPipeline
{
    public struct JsonFileData
    {
        public string JsonData { get; }

        public JsonFileData(string jsonData)
        {
            JsonData = jsonData;
        }
    }
}