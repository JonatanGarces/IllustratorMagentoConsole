using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole
{
    internal class EraseBak
    {


        //function to map the design's folder to extract:
        //the folders name is the category name
        //the file name contain the name of the product and the size of the artboard
        public List<localProduct> mapAiFolder(string path)
        {
            string[] allFolders = Directory.GetDirectories(path);
            List<localProduct> allFiles = new List<localProduct>();

            foreach (var folder in allFolders)
            {
                DirectoryInfo directory = new DirectoryInfo(folder);

                foreach (string file in Directory.GetFiles(directory.FullName, "*" + Constants.aiExtension))
                {
                    FileInfo info = new FileInfo(file);
                    localProduct productFromImage = decodeFileName(info);
                    //check if same image with different size has already been exported.
                    //localProduct repeatedProduct = allFiles.Find(localProduct => productFromImage.productName == localProduct.productName && localProduct.folderName == info.Directory.Name);
                    allFiles.Add(productFromImage);
                }
            }
            return allFiles;
        }

        public localProduct decodeFileName(FileInfo info)
        {
            string[] words = info.Name.Replace(Constants.aiExtension, "").Split('_');
            illustratorHelper ai = new illustratorHelper();

            if (words.Length > 0)
            {
                //check if it contains keyword na as not apply
                bool hasNa = words.Contains("na");
                bool isFunda = words.Contains("funda");

                if (!hasNa || !isFunda)
                {
                    localProduct png = new localProduct();
                    png.productName = words[0];
                    png.fileName = info.Name;
                    png.folderName = info.Directory.Name;
                    png.fullName = info.FullName;


                    var task = Task.Run(() =>
                    {
                        //string fullName, string folderNamem, string exportName = ""
                        return ai.exportAiToPng(png.fullName, png.folderName, png.productName);
                    });

                    bool success = task.Wait(TimeSpan.FromMilliseconds(6000));

                    if (success)
                    {
                        return png;
                    }
                    else
                    {
                        Console.WriteLine("NO exportado POR TIMEOUT:" + png.folderName + '|' + png.productName);
                        return null;
                    }
                }
            }
            return null;
        }


        /*
             public void updateCategoryTreeFromAiFolders()
    {
        List<Category> categories = getCategoryTree();

        illustratorHelper illustratorHelper = new illustratorHelper();
        string[] aiFolders = illustratorHelper.getFolders();
        foreach (var folder in aiFolders)
        {
            DirectoryInfo directory = new DirectoryInfo(folder);
            Category result = categories.Find(x => x.name == directory.Name);
            if (result == null)
            {
                Category category = new Category();

                category.name = directory.Name;
                category.parentId = 6;
                category.isActive = true;
                category.level = 0;
                category.position = 0;
                category.includeInMenu = false;
                createCategory(category);
            }
        }
    }
*/
        //OrderItem result = new OrderItem();
        //M2SalesOrderBillingAddress addressList = new M2SalesOrderBillingAddress();

        //M2Invoice invoice = JsonConvert.DeserializeObject<M2Invoice>(response.Content);
        //M2SalesOrder order = JsonConvert.DeserializeObject<M2SalesOrder>(response.Content);

        /*addressList = order.billing_address;
        List<OrderItem> data = new List<OrderItem>(order.items);

        foreach (var item in data)
        {
            result = item;
            Console.WriteLine(item.order_id);
        }
        */
        //M2Error m2Error = JsonConvert.DeserializeObject<M2Error>(response.Content);


        //MissingMemberHandling = MissingMemberHandling.Ignore,
        //DefaultValueHandling = DefaultValueHandling.Populate,

        /*
         public List<Category> getCategoryTree()
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/categories", Method.Get);
            request.AddParameter("rootCategoryId", 3); //config
            response = Client.Execute(request);
            Category categoryTree = JsonConvert.DeserializeObject<Category>(response.Content);
            return categoryTree.childrenData as List<Category>;
        }

        public Category getCategory(string folderName, int parentId)
        {
            //Console.WriteLine("Get category " + folderName + "with id: " + parentId);
            MagentoClient mc = new MagentoClient();
            List<Category> magentoCategories = mc.getCategories(folderName).items as List<Category>;
            Category magentoCategory = magentoCategories.Find(category => category.name == folderName);

            if (magentoCategory == null)
            {
                return createCategory(folderName, parentId);
            }
            else
            {
                return magentoCategory;
            }
        }

        public Category createCategory(string folderName, int parentId)
        {
            //Console.WriteLine("Creating category " + folderName + "with id: " + parentId);
            MagentoClient mc = new MagentoClient();
            Category magentoCategory = new Category();
            magentoCategory.name = folderName;
            magentoCategory.parentId = parentId;
            magentoCategory.isActive = true;
            magentoCategory.level = 0;
            magentoCategory.position = 0;
            magentoCategory.includeInMenu = false;
            return mc.createCategory(magentoCategory);
        }
        */
    }
}
