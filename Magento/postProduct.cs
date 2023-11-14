using Newtonsoft.Json;

namespace IllustratorMagentoConsole.Magento
{
    internal class postProduct
    {
        [JsonProperty("product")]
        public Product product { get; set; }

        [JsonProperty("saveOptions")]
        public bool saveOptions { get; set; }
    }
}