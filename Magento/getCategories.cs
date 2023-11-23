using Newtonsoft.Json;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.Magento
{
    internal class getCategories
    {
        [JsonProperty("items")]
        public IList<Category> items { get; set; }

        [JsonProperty("search_criteria")]
        public searchCriteria search_criteria { get; set; }

        [JsonProperty("total_count")]
        public int total_count { get; set; }
    }

}
