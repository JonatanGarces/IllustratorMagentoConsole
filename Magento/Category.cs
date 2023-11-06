using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole.Magento
{
    internal class Category
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("parent_id")]
        public int parentId { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("is_active")]
        public bool isActive { get; set; }

        [JsonProperty("level")]
        public int level { get; set; }

        [JsonProperty("position")]
        public int position { get; set; }

        [JsonProperty("product_count")]
        public int productCount { get; set; }

        [JsonProperty("include_in_menu")]
        public bool includeInMenu { get; set; }

        [JsonProperty("children_data")]
        public IList<Category> childrenData { get; set; }
    }
}