using Illustrator;
using IllustratorMagentoConsole.Magento;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IllustratorMagentoConsole
{
    internal class fileExplorer
    {
        MagentoClient mc = new MagentoClient();

        public Folder setFolderHierarchy(string root, int parentId)
        {
            Folder folder = new Folder();
            DirectoryInfo dir = new DirectoryInfo(root);
            folder.Name = dir.Name;
            folder.Path = root;
            folder.Files = dir.GetFiles().Where(
                fn => Constants.ExtensionWhitelist.Contains(fn.Extension)
                ).Select(
                x => getFileInFolder(x.FullName)
                ).ToList();

            Category magentoCategory = new Category();
            magentoCategory.name = dir.Name;
            magentoCategory.parentId = parentId;
            magentoCategory.isActive = true;
            magentoCategory.level = 0;
            magentoCategory.position = 0;
            magentoCategory.includeInMenu = false;

            folder.Category = mc.getCategory(magentoCategory);
            folder.SubFolders = dir.GetDirectories().Select(
                x => setFolderHierarchy(x.FullName, folder.Category.id)
                ).ToList();

            return folder;
        }

        public File getFileInFolder(string name)
        {
            File file = new File();
            FileInfo info = new FileInfo(name);
            file.Name = info.Name;
            file.Extension = info.Extension;
            file.Path = info.FullName;
            file.Product = getProduct(info);
            return file;
        }

        public Product getProduct(FileInfo info)
        {
            getAiInfo(info.FullName);
            Product product = new Product();
            Regex reg = new Regex("[ñ]");
            string sku = info.Name.Trim().Replace(" ", "-").ToLower();
            sku = reg.Replace(sku, "-");
            product.sku = sku;
            product.name = info.Name;
            product.price = 170;
            product.status = 1;
            product.visibility = 4;
            product.type_id = "simple";
            return mc.getProduct(product);
        }

        public class Folder
        {
            public string Path { set; get; }
            public string Name { set; get; }
            public List<File> Files { set; get; }
            public List<Folder> SubFolders { set; get; }
            public Category Category { set; get; }

        }

        public class File
        {
            public string Path { set; get; }
            public string Name { set; get; }
            public string Extension { set; get; }
            public Product Product { set; get; }
        }


        public void getAiInfo(string filePath)
        {
            Application illuApp = new Application();
            OpenOptions openOptions = new OpenOptions();
            Document illuDoc = illuApp.Open(filePath, AiDocumentColorSpace.aiDocumentCMYKColor);
            //illuDoc.Activate();
            //Artboards artBoards = illuDoc.Artboards;
            Console.WriteLine(illuDoc.Layers["producto"].GroupItems["imagen"].Name);
            illuDoc.Layers["producto"].GroupItems["imagen"].Selected = true;
            List<Layer> layers = illuDoc.Layers.Cast<Layer>().ToList();
            Layer layer = layers.Find(l => l.Name == "producto");
            if (layer != null)
            {
                illuApp.ExecuteMenuCommand("deselectall");

                //layer.GroupItems
                List<GroupItem> groupItems = layer.GroupItems.Cast<GroupItem>().ToList();
                GroupItem groupItem = groupItems.Find(tf => tf.Name.Trim().ToLower() == "imagen");
                if (groupItem != null)
                {
                    //illuDoc..Layers["producto"].GroupItems["imagen"].Selected = true;
                    //Console.WriteLine(illuDoc.GroupItems["imagen"]);
                    illuDoc.ExportSelectionAsPNG("C:\\Users\\Jonatan\\Documents\\1_Public\\1_Manganimeshon\\1_DTF-UV\\3_Marcas");
                    Console.WriteLine(groupItem.Name);

                }
                List<TextFrame> textFrames = layer.TextFrames.Cast<TextFrame>().ToList();
                string descripcion = textFrames.Find(tf => tf.Name.Trim().ToLower() == "descripcion").Contents ?? "";
                string peso = textFrames.Find(tf => tf.Name == "peso").Contents ?? "";
                string precio = textFrames.Find(tf => tf.Name == "precio").Contents ?? "";
                string sku = textFrames.Find(tf => tf.Name.Trim().ToLower() == "sku").Contents ?? "";
                string nombre = textFrames.Find(tf => tf.Name.Trim().ToLower() == "nombre").Contents ?? "";
                string tipo = textFrames.Find(tf => tf.Name.Trim().ToLower() == "tipo").Contents ?? "";


            }

        }
    }
}
