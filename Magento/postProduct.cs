using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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