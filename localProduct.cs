using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole
{
    internal class localProduct
    {
        public string productName { get; set; }
        public string fileName { get; set; }
        public string folderName { get; set; }
        public string filePath { get; set; }

        public string filePageSize { get; set; }
        public string fullName { get; set; }

        public string sku { get; set; }

        public int productId { get; set; }
        public int categoryId { get; set; }

        public string attributeId { get; set; }
    }
}
