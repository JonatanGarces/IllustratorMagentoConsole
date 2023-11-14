using Newtonsoft.Json;

namespace IllustratorMagentoConsole.Magento
{
    internal class ProductLinks
    {
        [JsonProperty("productLink")]
        public ProductLink productLink { get; set; }

        public class ProductLink
        {
            [JsonProperty("sku")]
            public string sku { get; set; }

            [JsonProperty("position")]
            public int position { get; set; }

            [JsonProperty("category_id")]
            public string category_id { get; set; }

            //[JsonProperty("extension_attributes")]
            //public ExtensionAttributes extension_attributes { get; set; }

            // public class ExtensionAttributes
            //{
            //}
        }
    }
}