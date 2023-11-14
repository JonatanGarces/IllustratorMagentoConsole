
using System;
using System.Collections.Generic;
using System.IO;

namespace IllustratorMagentoConsole
{
    internal class localProduct
    {
        public localProduct()
        {
            // Uncomment the following.
            filePath = Constants.exportadosFolder + Path.DirectorySeparatorChar +
                folderName + Path.DirectorySeparatorChar +
                Constants.fundasFolder + Path.DirectorySeparatorChar +
                this.productName + Constants.imageExtension;
        }
        //this is the name of the product without the extra info
        public string productName { get; set; }

        //name of the file, it contains the product name and extra info
        public string fileName { get; set; }

        //name of the directory in this case the category
        public string folderName { get; set; }
        public static HashSet<string> ExtensionWhitelist { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".ai" };

        public string filePath
        {
            get;

        }

        //full path of the file.
        public string fullName { get; set; }

        public string sku { get; set; }

        public int productId { get; set; }
        public int categoryId { get; set; }
        public string attributeId { get; set; }
    }
}
