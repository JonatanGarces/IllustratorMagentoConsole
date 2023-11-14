using IllustratorMagentoConsole.Magento;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace IllustratorMagentoConsole
{
    internal class Controller
    {
        public static void actualizarMagento()
        {

            fileExplorer fileExplorer = new fileExplorer();
            List<localProduct> productFolders = fileExplorer.mapAiFolder(Constants.clasificadosFolder);
            MagentoClient mc = new MagentoClient();
            List<Product> magentoProducts = mc.getProducts().items as List<Product>;
            List<AttributeOption> aom = mc.getAttribute("coleccion");
            string attribute_code = "coleccion";
            bool updateMediaGallery = true;
            int attribute_set_id = 4;
            int category_id = 3;
            foreach (var productFolder in productFolders)
            {
                AttributeOption attribute = aom.Find(option => option.label == productFolder.folderName);
                if (attribute == null)
                {
                    aom = mc.getAttribute("coleccion");
                    aom.Find(option => option.label == productFolder.folderName);
                    if (attribute == null)
                    {
                        attribute = new AttributeOption();
                        attribute.label = productFolder.folderName;
                        attribute.value = productFolder.folderName;
                        OptionAttribute op = new OptionAttribute()
                        {
                            option = attribute
                        };
                        string result = mc.createAttribute(attribute_code, op);
                        productFolder.attributeId = result.Trim();
                    }
                    else
                    {
                        productFolder.attributeId = attribute.value;
                    }
                }
                else
                {
                    productFolder.attributeId = attribute.value;
                }

                productFolder.categoryId = category_id;

                Console.WriteLine("category {0}", productFolder.folderName);

                //Sync Products
                Product magentoProduct = magentoProducts.Find(product => product.name == productFolder.productName);
                if (magentoProduct == null)
                {
                    Product product = new Product();
                    Regex reg = new Regex("[ñ]");
                    string sku = productFolder.productName.Trim().Replace(" ", "-").ToLower();
                    sku = reg.Replace(sku, "-");
                    product.sku = sku;
                    product.name = productFolder.productName;
                    product.attribute_set_id = attribute_set_id;
                    product.price = 170;
                    product.status = 1;
                    product.visibility = 4;
                    product.type_id = "simple";

                    Magento.Option option = new Magento.Option();
                    option.title = "Brand";
                    option.type = "field";
                    option.is_require = true;
                    option.price = 0;
                    option.price_type = "fixed";
                    option.product_sku = sku;

                    Magento.Option option1 = new Magento.Option();
                    option1.title = "Model";
                    option1.type = "field";
                    option1.is_require = true;
                    option1.price = 0;
                    option1.price_type = "fixed";
                    option1.product_sku = sku;

                    // List<Value> values = new List<Value>();
                    //option.values = valuesHelper();
                    product.options = new List<Magento.Option>() { option, option1 };
                    magentoProduct = mc.createProduct(product);
                }
                productFolder.productId = magentoProduct.id;
                productFolder.sku = magentoProduct.sku;

                CustomAttribute customAttr = magentoProduct.custom_attributes.Find(customAttribute => customAttribute.attribute_code == attribute_code);
                if (customAttr == null)
                {
                    Product product = new Product();
                    product.sku = productFolder.sku;
                    product.id = productFolder.productId;
                    product.attribute_set_id = attribute_set_id;
                    product.price = 170;
                    product.status = 1;
                    product.visibility = 4;
                    product.type_id = "simple";
                    List<CustomAttribute> customs = new List<CustomAttribute>();
                    CustomAttribute cattribute = new CustomAttribute();
                    cattribute.attribute_code = attribute_code;
                    cattribute.value = productFolder.attributeId;
                    customs.Add(cattribute);
                    product.custom_attributes = customs;

                    magentoProduct = mc.updateProduct(magentoProduct.sku, product);
                    customAttr = magentoProduct.custom_attributes.Find(customAttribute => customAttribute.attribute_code == attribute_code);
                }

                productFolder.attributeId = customAttr?.value == null ? "" : customAttr.value.ToString();
                Console.WriteLine("product {0}", productFolder.productName);
                List<ProductLinks.ProductLink> productLinks = mc.getProductLinks(productFolder.categoryId);
                ProductLinks.ProductLink productlink = productLinks.Find(productLink => productLink.sku == productFolder.sku);

                if (productlink == null)
                {
                    ProductLinks.ProductLink productLink = new ProductLinks.ProductLink();
                    productLink.sku = productFolder.sku;
                    productLink.category_id = productFolder.categoryId.ToString();
                    ProductLinks categoryLinks = new ProductLinks();
                    categoryLinks.productLink = productLink;
                    mc.createProductLink(categoryLinks);
                }

                List<media_gallery_entry> mediaGalleryEntries = mc.getMediaGaleryEntry(productFolder.sku);

                media_gallery_entry mediaGallery = (mediaGalleryEntries ?? new List<media_gallery_entry>() { }).Find(mediaGalleryEntry => mediaGalleryEntry.label == productFolder.productName);
                if (mediaGallery == null)
                {
                    media_gallery_entry media = new media_gallery_entry();
                    media.media_type = "image";
                    media.id = null;
                    //media.file = productFolder.productName + ".png";
                    media.label = productFolder.productName;
                    media.position = 0;
                    media.disabled = false;
                    media.types = new string[] { "image", "small_image", "thumbnail", "swatch_image" };
                    media_content mediaContent = new media_content();
                    Console.WriteLine(media.file);

                    using (Image image = Image.FromFile(productFolder.filePath))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            // Convert byte[] to Base64 String
                            string base64String = Convert.ToBase64String(imageBytes);
                            mediaContent.base64_encoded_data = base64String;
                        }
                    }
                    mediaContent.type = "image/png";
                    mediaContent.name = productFolder.productName;
                    media.content = mediaContent;
                    Entry entry = new Entry();
                    entry.entry = media;
                    Console.WriteLine("media gallery creation {0}", productFolder.productName);
                    mc.createEntryMediaGallery(productFolder.sku, entry);
                    Console.WriteLine("media gallery uploaded {0}", productFolder.productName);
                }
                else if (updateMediaGallery)
                {
                    media_gallery_entry media = new media_gallery_entry();
                    media.media_type = "image";
                    //media.label = productFolder.productName;
                    media.position = 1;
                    media.disabled = false;
                    media.types = new string[] { "image", "small_image", "thumbnail" };
                    media.id = mediaGallery.id;
                    media_content mediaContent = new media_content();

                    using (Image image = Image.FromFile(productFolder.filePath))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            string base64String = Convert.ToBase64String(imageBytes);
                            mediaContent.base64_encoded_data = base64String;
                        }
                    }
                    mediaContent.type = "image/png";
                    mediaContent.name = productFolder.productName;
                    media.content = mediaContent;
                    Entry entry = new Entry();
                    entry.entry = media;
                    Console.WriteLine("media gallery update {0}", productFolder.productName);
                    mc.updateEntryMediaGallery(productFolder.sku, mediaGallery.id, entry);
                    Console.WriteLine("media gallery uploaded {0}", productFolder.productName);
                }
            }
            Console.ReadLine();
        }

        public static void eliminarProductos()
        {
            fileExplorer fileExplorer = new fileExplorer();
            List<localProduct> productFolders = fileExplorer.mapAiFolder(Constants.clasificadosFolder);
            MagentoClient mc = new MagentoClient();
            List<Product> magentoProducts = mc.getProducts().items as List<Product>;
            foreach (var magentoProduct in magentoProducts)
            {
                localProduct product = productFolders.Find(productFolder => productFolder.productName == magentoProduct.name);
                if (product == null)
                {
                    Console.WriteLine("Producto Borrado {0}", magentoProduct.sku);
                    mc.deleteProduct(magentoProduct.sku);
                }
            }
            Console.ReadLine();
        }

        public static List<Value> valuesHelper()
        {
            List<Value> values = new List<Value>();
            Console.WriteLine("fixed");
            Value value = new Value();
            //value.title = "Marca";
            //value.sort_order = 1;
            value.price = 0;
            value.price_type = "fixed";
            values.Add(value);
            /*
            //
            value = new Value();
            value.title = "A9 (37 x 52) mm";
            value.sort_order = 3;
            value.price = 4;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A8 (52 x 74) mm";
            value.sort_order = 4;
            value.price = 5;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A7 (74 x 105) mm";
            value.sort_order = 5;
            value.price = 8;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A6 (105 x 148) mm";
            value.sort_order = 6;
            value.price = 15;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A5 (148 x 297) mm";
            value.sort_order = 7;
            value.price = 30;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A4 (210 x 297) mm";
            value.sort_order = 8;
            value.price = 60;
            value.price_type = "fixed";
            values.Add(value);
            //
            value = new Value();
            value.title = "A3 (297 x 420) mm";
            value.sort_order = 9;
            value.price = 120;
            value.price_type = "fixed";
            values.Add(value);
            */
            return values;
        }

    }
}
