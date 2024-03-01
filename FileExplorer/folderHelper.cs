using Illustrator;
using IllustratorMagentoConsole.FileExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IllustratorMagentoConsole
{
    internal class folderHelper
    {
        public List<ProductInfo> products;

        public folderHelper()
        {
            products = new List<ProductInfo>();
        }
        public CategoryFolder setFolderHierarchy(string root)
        {
            CategoryFolder folder = new CategoryFolder();
            DirectoryInfo dir = new DirectoryInfo(root);
            folder.Name = dir.Name;
            folder.Path = root;
            folder.SubFolders = dir.GetDirectories().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).Select(
               x => setFolderHierarchy(x.FullName)
               ).ToList();
            folder.Files = dir.GetFiles().Where(
                fn => Constants.ExtensionWhitelist.Contains(fn.Extension)
                ).Select(
                x => getFileInFolder(x.FullName)
                ).ToList();

            return folder;
        }

        public ProductFile getFileInFolder(string name)
        {
            FileInfo info = new FileInfo(name);
            ProductFile file = new ProductFile();

            file.Name = info.Name.Replace(info.Extension, "");
            file.FullName = info.FullName;
            file.Extension = info.Extension;
            file.Path = info.Directory.FullName;
            ProductInfo productInfo = illustratorInfo(file);
            if (productInfo != null)
            {
                products.Add(productInfo);
            }
            file.productInfo = illustratorInfo(file);
            return file;
        }

        public ProductInfo illustratorInfo(ProductFile info)
        {
            Application illuApp = new Application();
            OpenOptions openOptions = new OpenOptions();
            try
            {
                Document illuDoc = illuApp.Open(info.FullName, AiDocumentColorSpace.aiDocumentCMYKColor);

                try
                {
                    illuDoc.Activate();
                    illuApp.ExecuteMenuCommand("deselectall");

                    List<Layer> layers = illuDoc.Layers.Cast<Layer>().ToList();

                    Layer layer = layers.Find(tf => tf.Name.Trim().ToLower() == "producto");
                    if (layer != null)
                    {
                        illuDoc.Layers["producto"].Locked = false;
                        List<GroupItem> groupItems = layer.GroupItems.Cast<GroupItem>().ToList();
                        GroupItem groupItem = groupItems.Find(tf => tf.Name.Trim().ToLower() == "imagen");
                        if (groupItem != null)
                        {
                            String pngPath = info.Path + Path.DirectorySeparatorChar + info.Name.Replace(" ", "-") + Constants.imageExtension;
                            //Console.WriteLine($"{pngPath}");
                            groupItem.Selected = true;
                            illuDoc.ExportSelectionAsPNG(pngPath);
                            List<TextFrame> textFrames = layer.TextFrames.Cast<TextFrame>().ToList();
                            ProductInfo product = new ProductInfo();
                            product.descripcion = textFrames.Find(tf => tf.Name.Trim().ToLower() == "descripcion")?.Contents ?? "";
                            product.precio = Convert.ToDouble(textFrames.Find(tf => tf.Name == "precio")?.Contents);
                            product.peso = Convert.ToDouble(textFrames.Find(tf => tf.Name == "peso")?.Contents);
                            product.nombre = textFrames.Find(tf => tf.Name.Trim().ToLower() == "nombre")?.Contents ?? "";
                            product.tipo = textFrames.Find(tf => tf.Name.Trim().ToLower() == "tipo")?.Contents ?? "";
                            product.modelo = textFrames.Find(tf => tf.Name.Trim().ToLower() == "modelo")?.Contents ?? "";
                            string categoriesPath = info.Path.Replace(Constants.diseñosManganimeshon + Path.DirectorySeparatorChar, "");
                            string[] categories = categoriesPath.Split('\\');
                            if (0 < categories.Length) { product.genre = categories[0]; }
                            if (1 < categories.Length) { product.collection = categories[1]; }
                            if (2 < categories.Length) { product.vendor = categories[2]; }
                            if (3 < categories.Length) { product.tag = categories[3]; }
                            product.categories = categories;
                            //Console.WriteLine(product.collection);
                            //Console.WriteLine(product.vendor);
                            //Console.WriteLine(product.tag);
                            product.imagePath = pngPath;
                            illuDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
                            return product;
                        }
                        else
                        {
                            this.errorsLog(info.Name + ": No tiene etiqueta imagen");
                        }
                    }
                    else
                    {
                        this.errorsLog(info.Name + ": No tiene capa producto");
                    }
                }
                catch (Exception e)
                {

                    this.errorsLog(info.Name);
                    this.errorsLog(e.Message);
                }
                illuDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
            }
            catch (Exception e)
            {

                this.errorsLog(info.Name);
                this.errorsLog(e.Message);
            }
            return null;
        }

        public void errorsLog(String mensaje)
        {
            using (StreamWriter w = System.IO.File.AppendText(@"C:\Users\Jonatan\Documents\1_Public\1_Manganimeshon\1_DTF-UV\3_Marcas\illustrator.txt"))
            {
                w.WriteLine(mensaje);
            }
        }

    }
}
