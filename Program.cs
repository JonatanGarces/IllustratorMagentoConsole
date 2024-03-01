using IllustratorMagentoConsole.FileExplorer;
using System;

namespace IllustratorMagentoConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //folderHelper fe = new folderHelper();
            //ShopifyClient sc = new ShopifyClient();
            // List<ShopifySharp.Product> shopifyProducts = sc.getProductsAsync().Result;
            CategoryFolder folder = new fileExplorer().setFolderHierarchy(Constants.diseñosManganimeshon, 1); // 1 root 3 3_Marcas 51

            Console.ReadLine();




            //foreach (var product in fe.products)
            //{
            //    Console.WriteLine(product.imagePath);
            // }

            /*
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Jonatan\Documents\1_Public\1_Manganimeshon\1_DTF-UV\3_Marcas\json.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, fe.errorFiles);
            }
            */

        }
    }
}
// {"ExpiryDate":new Date(1230375600000),"Price":0}
