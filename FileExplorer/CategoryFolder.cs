using IllustratorMagentoConsole.Magento;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.FileExplorer
{
    internal class CategoryFolder
    {
        public string Path { set; get; }
        public string Name { set; get; }
        public List<ProductFile> Files { set; get; }
        public List<CategoryFolder> SubFolders { set; get; }
        public Category Category { set; get; }
    }
}
