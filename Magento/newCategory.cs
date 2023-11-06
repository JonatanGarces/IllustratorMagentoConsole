using IllustratorMagentoConsole.Magento;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole.Magento
{
    internal class newCategory
    {
        [JsonProperty("category")]
        public Category category { get; set; }
    }
}