using Illustrator;
using IllustratorMagentoConsole.FileExplorer;
using IllustratorMagentoConsole.Magento;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IllustratorMagentoConsole
{
    internal class fileExplorer
    {
        MagentoClient mc = new MagentoClient();
        public List<string> errorFiles = new List<string>();

        public CategoryFolder setFolderHierarchy(string root, int parentId = 0)
        {
            CategoryFolder folder = new CategoryFolder();
            DirectoryInfo dir = new DirectoryInfo(root);
            folder.Name = dir.Name;
            folder.Path = root;
            Category magentoCategory = new Category();
            magentoCategory.name = dir.Name;
            magentoCategory.parentId = parentId;
            magentoCategory.isActive = true;
            magentoCategory.level = 0;
            magentoCategory.position = 0;
            magentoCategory.includeInMenu = false;
            folder.Category = mc.getCategory(magentoCategory);
            folder.SubFolders = dir.GetDirectories().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).Select(
                x => setFolderHierarchy(x.FullName, folder.Category.id)
                ).ToList();
            folder.Files = dir.GetFiles().Where(
                fn => Constants.ExtensionWhitelist.Contains(fn.Extension)
                ).Select(
                x => getFileInFolder(x.FullName, folder.Category.id)
                ).ToList();

            return folder;
        }

        public ProductFile getFileInFolder(string name, int categoryId)
        {
            ProductFile file = new ProductFile();
            FileInfo info = new FileInfo(name);
            file.Name = info.Name.Replace(info.Extension, "");
            file.FullName = info.FullName;
            file.Extension = info.Extension;
            file.Path = info.Directory.FullName;
            file.CategoryId = categoryId;
            var product = getProduct(file);
            file.Product = product;
            if (product == null)
            {
                Console.WriteLine("miados");
                return file;

            }
            Console.WriteLine(product.sku);
            file.Sku = product.sku;

            file.Media = getMedia(file, file.Product);
            return file;

        }

        public Product getProduct(ProductFile info)
        {
            Product product = extractProductInfoFromIllustratorFile(info);
            if (product == null)
            {
                Console.WriteLine("kgods");
                return null;
            }
            return mc.getProduct(product);

        }

        public media_gallery_entry getMedia(ProductFile info, Product productInfo)
        {
            media_gallery_entry media = new media_gallery_entry();
            media.media_type = "image";
            media.label = info.Name;
            media.position = 1;
            media.disabled = false;
            media.types = new string[] { "image", "small_image", "thumbnail" };
            //media.id = mediaGallery.id;
            media_content mediaContent = new media_content();
            if (productInfo == null)
            {
                Console.WriteLine("productinfo null");
                return null;
            }
            try
            {
                Console.WriteLine(info.Path + Path.DirectorySeparatorChar + info.Sku + Constants.imageExtension);
                using (Image image = Image.FromFile(info.Path + Path.DirectorySeparatorChar + info.Sku + Constants.imageExtension))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        //m.Close();
                        //m.Flush();
                        //Convert byte[] to Base64 String
                        mediaContent.base64_encoded_data = Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            mediaContent.type = "image/png";
            mediaContent.name = info.Sku + Constants.imageExtension;
            media.content = mediaContent;
            Entry entry = new Entry();
            entry.entry = media;
            media = mc.getEntryMediaGallery(info.Sku, entry);
            return media;
        }

        public Product extractProductInfoFromIllustratorFile(ProductFile info)
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
                            groupItem.Selected = true;
                            List<TextFrame> textFrames = layer.TextFrames.Cast<TextFrame>().ToList();
                            Product product = new Product();
                            product.custom_attributes = new List<CustomAttribute>() {
                            new CustomAttribute
                            {
                                attribute_code = "description",
                                value = textFrames.Find(tf => tf.Name.Trim().ToLower() == "descripcion")?.Contents
                            },
                            new CustomAttribute
                            {
                                attribute_code = "short_description",
                                value = textFrames.Find(tf => tf.Name.Trim().ToLower() == "descripcion")?.Contents
                            },
                            new CustomAttribute
                            {
                                attribute_code = "category_ids",
                                value = new List<int>(){ info.CategoryId }
                            }
                        };

                            product.price = Convert.ToDouble(textFrames.Find(tf => tf.Name == "precio")?.Contents);
                            product.weight = Convert.ToDouble(textFrames.Find(tf => tf.Name == "peso")?.Contents);
                            //string sku = textFrames.Find(tf => tf.Name.Trim().ToLower() == "sku")?.Contents ?? "";
                            string name = (textFrames.Find(tf => tf.Name.Trim().ToLower() == "nombre")?.Contents ?? "").Trim();
                            string tipo = (textFrames.Find(tf => tf.Name.Trim().ToLower() == "tipo")?.Contents ?? "").Trim();
                            string nameTipo = FirstCharSubstring(tipo) + " " + FirstCharSubstring(name);
                            product.name = nameTipo;
                            product.status = 1;
                            product.sku = new Regex("[ñ]").Replace(nameTipo.Trim().Replace(" ", "-").ToLower(), "-");
                            //Catalog and search
                            product.visibility = 4;
                            product.attribute_set_id = 4;
                            product.type_id = "simple";
                            illuDoc.ExportSelectionAsPNG(info.Path + Path.DirectorySeparatorChar + product.sku + Constants.imageExtension);

                            illuDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
                            return product;
                        }
                        else
                        {
                            this.errorsLog(info.Name + ": No tiene imagen");
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

        public string FirstCharSubstring(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return $"{input[0].ToString().ToUpper()}{input.Substring(1)}";
        }


        public void errorsLog(String mensaje)
        {
            using (StreamWriter w = System.IO.File.AppendText(@"C:\Users\Jonatan\Documents\1_Public\1_Manganimeshon\1_DTF-UV\3_Marcas\illustrator.txt"))
            {
                w.WriteLine(mensaje);
            }
        }

        public void log()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Jonatan\Documents\1_Public\1_Manganimeshon\1_DTF-UV\3_Marcas\json.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, errorFiles);
            }
        }

    }
}
