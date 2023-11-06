using IllustratorMagentoConsole.Magento;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole.Magento
{
    internal class MagentoClient
    {
        public static RestClient Client { get; set; }
        public static string token { get; set; }
        public static string baseUrl { get; set; }
        public static string username { get; set; }
        public static string password { get; set; }

        public MagentoClient()
        {
            baseUrl = baseUrl != null ? baseUrl : "http://192.168.1.79:4080";//"https://manganimeshon.com";config
            username = username != null ? username : "user"; //"admin";config
            password = password != null ? password : "bitnami1"; //"o48098Eh";config
            Client = Client != null ? Client : new RestClient(baseUrl);
            token = token != null && token != "" ? token : GetAdminToken().Replace("\"", "").Trim();
        }

        public getProducts getProducts()
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products", Method.Get);
            request.AddQueryParameter("searchCriteria[pageSize]", 200);
            request.AddQueryParameter("searchCriteria[currentPage]", 1);
            response = Client.Execute(request);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                //MissingMemberHandling = MissingMemberHandling.Ignore,
                //DefaultValueHandling = DefaultValueHandling.Populate,
                Error = (sender, error) => error.ErrorContext.Handled = true
            };
            getProducts getProducts = JsonConvert.DeserializeObject<getProducts>(response.Content, settings);
            return getProducts;
        }

        public List<ProductLinks.ProductLink> getProductLinks(int categoryId)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/categories/" + categoryId + "/products", Method.Get);
            response = Client.Execute(request);
            try
            {
                List<ProductLinks.ProductLink> productLinks = JsonConvert.DeserializeObject<List<ProductLinks.ProductLink>>(response.Content);
                return productLinks;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return null;
        }

        public List<media_gallery_entry> getMediaGaleryEntry(string sku)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/" + sku + "/media", Method.Get);
            response = Client.Execute(request);
            try
            {
                List<media_gallery_entry> productLinks = JsonConvert.DeserializeObject<List<media_gallery_entry>>(response.Content);
                return productLinks;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return null;
        }

        public List<AttributeOption> getAttribute(string attribute_code)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/attributes/" + attribute_code + "/options", Method.Get);
            response = Client.Execute(request);
            try
            {
                List<AttributeOption> productLinks = JsonConvert.DeserializeObject<List<AttributeOption>>(response.Content);
                return productLinks;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return null;
        }

        public string createAttribute(string attribute_code, OptionAttribute entry)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/attributes/" + attribute_code + "/options", Method.Post);
            try
            {
                var json = JsonConvert.SerializeObject(entry, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return "";
        }

        public bool createEntryMediaGallery(string sku, Entry entry)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/" + sku + "/media", Method.Post);
            try
            {
                var json = JsonConvert.SerializeObject(entry, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool updateEntryMediaGallery(string sku, int? entryId, Entry entry)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/" + sku + "/media/" + entryId, Method.Put);
            try
            {
                var json = JsonConvert.SerializeObject(entry, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return false;
        }

        public bool createProductLink(ProductLinks productLink)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/categories/" + productLink.productLink.category_id + "/products", Method.Put);
            try
            {
                var json = JsonConvert.SerializeObject(productLink, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);
                return Convert.ToBoolean(response.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return false;
        }

        public Product createProduct(Product product)
        {
            Product productResponse = null;
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products", Method.Post);
            try
            {
                postProduct postProduct = new postProduct();
                postProduct.product = product;
                postProduct.saveOptions = true;

                var json = JsonConvert.SerializeObject(postProduct, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    //MissingMemberHandling = MissingMemberHandling.Ignore,
                    //DefaultValueHandling = DefaultValueHandling.Populate,
                    Error = (sender, error) => error.ErrorContext.Handled = true
                };
                productResponse = JsonConvert.DeserializeObject<Product>(response.Content, settings);
                return productResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(response.Content);
            }
            return productResponse;
        }

        public Product updateProduct(string sku, Product product)
        {
            Product productResponse = null;
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/" + sku, Method.Put);
            try
            {
                postProduct postProduct = new postProduct();
                postProduct.product = product;
                postProduct.saveOptions = true;

                var json = JsonConvert.SerializeObject(postProduct, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                response = Client.Execute(request);
                productResponse = JsonConvert.DeserializeObject<Product>(response.Content);
                return productResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return productResponse;
        }

        public bool deleteProduct(string sku)
        {
            bool productResponse = false;
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/products/" + sku, Method.Delete);
            try
            {
                response = Client.Execute(request);
                return Convert.ToBoolean(response.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return productResponse;
        }

        public Category createCategory(Category category)
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/categories", Method.Post);
            newCategory newCategory = new newCategory();
            newCategory.category = category;
            var json = JsonConvert.SerializeObject(newCategory, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            response = Client.Execute(request);
            Category categoryTree = JsonConvert.DeserializeObject<Category>(response.Content);

            return categoryTree;
        }

        public List<Category> getCategoryTree()
        {
            RestResponse response = null;
            var request = CreateRequest("/rest/V1/categories", Method.Get);
            request.AddParameter("rootCategoryId", 3); //config
            response = Client.Execute(request);
            Category categoryTree = JsonConvert.DeserializeObject<Category>(response.Content);
            return categoryTree.childrenData as List<Category>;
        }

        private static string GetAdminToken()
        {
            RestResponse response = null;
            try
            {
                RestRequest request = CreateRequest("/rest/V1/integration/admin/token", Method.Post);
                var user = new UserCredential();
                user.username = username;
                user.password = password;

                var json = JsonConvert.SerializeObject(user);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                response = Client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Content;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return "";
        }

        public static RestRequest CreateRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.RequestFormat = DataFormat.Json;
            if (token != "" && token != null)
            {
                request.AddHeader("Authorization", "Bearer " + token);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Accept", "application/json");
            }
            return request;
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
    }
}