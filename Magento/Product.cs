using Newtonsoft.Json;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.Magento
{
    internal class Product
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("sku")]
        public string sku { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("attribute_set_id")]
        public int attribute_set_id { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("visibility")]
        public int visibility { get; set; }

        [JsonProperty("type_id")]
        public string type_id { get; set; }

        [JsonProperty("created_at")]
        public string created_at { get; set; }

        [JsonProperty("extension_attributes")]
        public ExtensionAttributes extension_attributes { get; set; }

        [JsonProperty("updated_at")]
        public string updated_at { get; set; }

        [JsonProperty("weight")]
        public double weight { get; set; }

        [JsonProperty("media_gallery_entries")]
        public IList<media_gallery_entry> media_gallery_entries { get; set; }

        [JsonProperty("options")]
        public IList<Option> options { get; set; }

        [JsonProperty("custom_attributes")]
        public List<CustomAttribute> custom_attributes { get; set; }
    }

    internal class CustomAttribute
    {
        [JsonProperty("attribute_code")]
        public string attribute_code { get; set; }

        [JsonProperty("value")]
        public object value { get; set; }
    }

    internal class Entry
    {
        [JsonProperty("entry")]
        public media_gallery_entry entry { get; set; }
    }

    internal class media_gallery_entry
    {
        [JsonProperty("id")]
        public int? id { get; set; }

        [JsonProperty("media_type")]
        public string media_type { get; set; }

        [JsonProperty("label")]
        public string label { get; set; }

        [JsonProperty("position")]
        public int position { get; set; }

        [JsonProperty("disabled")]
        public bool disabled { get; set; }

        [JsonProperty("types")]
        public string[] types { get; set; }

        [JsonProperty("content")]
        public media_content content { get; set; }

        [JsonProperty("file")]
        public string file { get; set; }
    }

    internal class media_content
    {
        [JsonProperty("base64_encoded_data")]
        public string base64_encoded_data { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }
    }

    internal class ExtensionAttributes
    {
        [JsonProperty("website_ids")]
        public int[] website_ids { get; set; }

        [JsonProperty("category_links")]
        public List<ProductLinks.ProductLink> category_links { get; set; }
    }

    internal class Option
    {
        [JsonProperty("product_sku")]
        public string product_sku { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("is_require")]
        public bool is_require { get; set; }

        [JsonProperty("values")]
        public List<Value> values { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("price_type")]
        public string price_type { get; set; }
    }

    internal class Value
    {
        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("sort_order")]
        public int sort_order { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("price_type")]
        public string price_type { get; set; }
    }
}