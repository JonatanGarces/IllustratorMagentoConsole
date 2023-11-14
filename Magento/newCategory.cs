using Newtonsoft.Json;

namespace IllustratorMagentoConsole.Magento
{
    internal class newCategory
    {
        [JsonProperty("category")]
        public Category category { get; set; }
    }
}