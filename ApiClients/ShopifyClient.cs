using Newtonsoft.Json;
using ShopifySharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole.ApiClients
{
    internal class ShopifyClient
    {
        public static string url { get; set; }
        public static string token { get; set; }

        public ShopifyClient()
        {
            url = Constants.Shopify.url;
            token = Constants.Shopify.token;
            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore, //DefaultValueHandling.Populate,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        public async Task<List<Product>> getProductsAsync(string title = "")
        {
            ProductService service = new ProductService(url, token);
            var allProducts = new List<ShopifySharp.Product>();
            var filters = new ShopifySharp.Filters.ProductListFilter()
            {
                Limit = 250
            };

            //Optionally filter the results
            var page = await service.ListAsync(filters);

            while (true)
            {
                allProducts.AddRange(page.Items);
                if (!page.HasNextPage)
                {
                    // We've reached the end of the list
                    break;
                }
                // There is at least one more page, list it and loop again
                page = await service.ListAsync(page.GetNextPageFilter());
            }

            return allProducts;
        }

        public async Task<Product> createProductAsync(ShopifySharp.Product product)
        {
            var service = new ProductService(url, token);
            return await service.CreateAsync(product);
        }

        /*
        foreach (var item in allProducts)
        {

            //ProductImageService productimageservice = new ProductImageService(url, token);
            //var images = await productimageservice.ListAsync((long)item.Id);
            //if (!(images.Items.Count() > 0))
            //{
            //}

            Console.WriteLine(item.Title);
            Console.WriteLine(item.Id);
            Console.WriteLine(item.Status);
            Console.WriteLine(item.Vendor);
            Console.WriteLine(item.Tags);
            Console.WriteLine(item.Metafields);
            Console.WriteLine(item.BodyHtml);
            Console.WriteLine(item.ProductType);

            if (item.Metafields != null)
            {
                foreach (var meta in item.Metafields)
                {
                    Console.WriteLine(meta.ToString());
                }


            }
            Console.WriteLine("-------------");
        }
        var product = new Product()
        {
        Title = "Burton Custom Freestlye 151",
        Vendor = "Burton",
        BodyHtml = "<strong>Good snowboard!</strong>",
        ProductType = "Snowboard",
        Images = new List<ProductImage>
        {
        new ProductImage
        {
        Attachment = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw=="
        }
        },
        };
        */

    }
}
