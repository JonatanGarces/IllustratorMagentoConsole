using IllustratorMagentoConsole.Magento;

namespace IllustratorMagentoConsole.FileExplorer
{
    internal class ProductFile
    {

        public string Sku { set; get; }
        public string Name { set; get; }
        public string FullName { set; get; }
        public string Extension { set; get; }
        public string Path { set; get; }
        public int CategoryId { set; get; }
        public Product Product { set; get; }
        public ProductInfo productInfo { set; get; }
        public media_gallery_entry Media { set; get; }

    }
}
