using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.Magento
{
    internal class MagentoClient
    {
        public static RestClient Client { get; set; }
        public static string token { get; set; }

        public MagentoClient()
        {
            Client = Client != null ? Client : new RestClient(Constants.Magento.baseUrl);
            token = token != null && token != "" ? token : GetAdminToken().Replace("\"", "").Trim();
            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore, //DefaultValueHandling.Populate,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        public List<media_gallery_entry> getMediaGaleryEntry(string sku)
        {
            var response = CreateRequest("/rest/V1/products/" + sku + "/media", Method.Get);
            return JsonConvert.DeserializeObject<List<media_gallery_entry>>(response) ?? null;
        }

        public List<AttributeOption> getAttribute(string attribute_code)
        {
            var response = CreateRequest("/rest/V1/products/attributes/" + attribute_code + "/options", Method.Get);
            return JsonConvert.DeserializeObject<List<AttributeOption>>(response) ?? null;
        }

        public string createAttribute(string attribute_code, OptionAttribute entry)
        {
            return CreateRequest("/rest/V1/products/attributes/" + attribute_code + "/options", Method.Post, entry) ?? "";
        }

        public Entry createEntryMediaGallery(string sku, Entry entry)
        {
            var response = CreateRequest("/rest/V1/products/" + sku + "/media", Method.Post, entry);
            return JsonConvert.DeserializeObject<Entry>(response) ?? null;

        }

        public Entry updateEntryMediaGallery(string sku, int? entryId, Entry entry)
        {
            var response = CreateRequest("/rest/V1/products/" + sku + "/media/" + entryId, Method.Put, entry);
            return JsonConvert.DeserializeObject<Entry>(response) ?? null;
        }
        public List<ProductLinks.ProductLink> getProductLinks(int categoryId)
        {
            var response = CreateRequest("/rest/V1/categories/" + categoryId + "/products", Method.Get);
            return JsonConvert.DeserializeObject<List<ProductLinks.ProductLink>>(response) ?? null;
        }

        public ProductLinks createProductLink(ProductLinks productLink)
        {
            var json = JsonConvert.SerializeObject(productLink, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Error = (sender, error) => error.ErrorContext.Handled = true

            });
            var response = CreateRequest("/rest/V1/categories/" + productLink.productLink.category_id + "/products", Method.Put, productLink);
            return JsonConvert.DeserializeObject<ProductLinks>(response) ?? null;
        }


        public Product getProduct(Product fileProduct)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][field]", "name"));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][value]", fileProduct.name));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][conditionType]", "eq"));
            var response = CreateRequest("/rest/V1/products", Method.Get, null, queryParameters);
            //Console.WriteLine(response);

            List<Product> products = ((JsonConvert.DeserializeObject<getProducts>(response).items) ?? null) as List<Product>;
            Product product = products.Find(p => p.name == fileProduct.name) ?? null;

            return null;
            if (product != null)
            {
                return product;
            }
            return createProduct(fileProduct);
        }
        public Category getCategory(Category folderCategory)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][field]", "name"));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][value]", folderCategory.name));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][conditionType]", "eq"));
            var response = CreateRequest("/rest/V1/categories/list", Method.Get, null, queryParameters);
            List<Category> categories = ((JsonConvert.DeserializeObject<getCategories>(response).items) ?? null) as List<Category>;
            Category category = categories.Find(c => c.name == folderCategory.name) ?? null;

            if (category != null)
            {
                return category;
            }
            return createCategory(folderCategory);
        }

        public Product createProduct(Product product)
        {
            postProduct postProduct = new postProduct();
            postProduct.product = product;
            postProduct.saveOptions = true;
            var response = CreateRequest("/rest/V1/products", Method.Post, postProduct);
            return JsonConvert.DeserializeObject<Product>(response) ?? null;
        }

        public Product updateProduct(Product product)
        {
            postProduct postProduct = new postProduct();
            postProduct.product = product;
            postProduct.saveOptions = true;
            var response = CreateRequest("/rest/V1/products/" + product.sku, Method.Put, postProduct);
            return JsonConvert.DeserializeObject<Product>(response) ?? null;
        }

        public getProducts getProducts()
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter("searchCriteria[pageSize]", "200"));
            queryParameters.Add(new QueryParameter("searchCriteria[currentPage]", "1"));
            var response = CreateRequest("/rest/V1/products", Method.Get, null, queryParameters);
            return JsonConvert.DeserializeObject<getProducts>(response) ?? null;
        }

        public bool deleteProduct(string sku)
        {
            var response = CreateRequest("/rest/V1/products/" + sku, Method.Delete);
            return Convert.ToBoolean(response);
        }

        public getCategories getCategories(string name)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][field]", "name"));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][value]", name));
            queryParameters.Add(new QueryParameter("searchCriteria[filterGroups][0][filters][0][conditionType]", "eq"));
            var response = CreateRequest("/rest/V1/categories/list", Method.Get, null, queryParameters);
            return JsonConvert.DeserializeObject<getCategories>(response);
        }

        public Category createCategory(Category category)
        {
            newCategory newCategory = new newCategory();
            newCategory.category = category;
            var response = CreateRequest("/rest/V1/categories", Method.Post, newCategory);
            return JsonConvert.DeserializeObject<Category>(response) ?? null;
        }

        private static string GetAdminToken()
        {
            UserCredential user = new UserCredential();
            user.username = Constants.Magento.username;
            user.password = Constants.Magento.password;
            return CreateRequest("/rest/V1/integration/admin/token", Method.Post, user) ?? "";
        }

        public static string CreateRequest(string endpoint, Method method, object jsonObject = null, List<QueryParameter> queryParameters = null)
        {
            var request = new RestRequest(endpoint, method);
            request.RequestFormat = DataFormat.Json;
            if (token != "" && token != null)
            {
                request.AddHeader("Authorization", "Bearer " + token);
            }
            if (jsonObject != null)
            {
                var jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Error = (sender, error) => error.ErrorContext.Handled = true
                });
                request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            }

            if (queryParameters != null)
            {
                queryParameters.ForEach(x => request.AddQueryParameter(x.Name, (string)x.Value));
            }
            try
            {
                var response = Client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Content;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }


    }
}