using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole.Magento
{
    internal class OptionAttribute
    {
        [JsonProperty("option")]
        public AttributeOption option { get; set; }
    }

    internal class AttributeOption
    {
        [JsonProperty("label")]
        public string label { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }

        [JsonProperty("sort_order")]
        public int sort_order { get; set; }

        [JsonProperty("is_default")]
        public bool is_default { get; set; }

        [JsonProperty("store_labels")]
        public List<StoreLabel> store_labels { get; set; }

        internal class StoreLabel
        {
            [JsonProperty("store_id")]
            public int store_id { get; set; }

            [JsonProperty("label")]
            public string label { get; set; }
        }
    }
}