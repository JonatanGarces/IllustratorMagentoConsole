using Newtonsoft.Json;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.Magento
{
    internal class getProducts
    {
        [JsonProperty("items")]
        public IList<Product> items { get; set; }

        [JsonProperty("search_criteria")]
        public searchCriteria search_criteria { get; set; }

        [JsonProperty("total_count")]
        public int total_count { get; set; }
    }

    internal class searchCriteria
    {
        [JsonProperty("current_page")]
        public int current_page { get; set; }

        [JsonProperty("filter_groups")]
        public IList<filterGroup> filter_groups { get; set; }

        [JsonProperty("page_size")]
        public int page_size { get; set; }

        [JsonProperty("sort_orders")]
        public IList<SortOrder> sort_orders { get; set; }
    }

    internal class filterGroup
    {
        [JsonProperty("condition_type")]
        public string condition_type { get; set; }

        [JsonProperty("field")]
        public string field { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }
    }

    internal class SortOrder
    {
        [JsonProperty("direction")]
        public string direction { get; set; }

        [JsonProperty("field")]
        public string field { get; set; }
    }
}