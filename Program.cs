using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using static IllustratorMagentoConsole.fileExplorer;

namespace IllustratorMagentoConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            fileExplorer fe = new fileExplorer();
            Folder folder = fe.setFolderHierarchy(Constants.diseñosManganimeshon);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Jonatan\Documents\1_Public\1_Manganimeshon\1_DTF-UV\3_Marcas\json.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, folder);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }
            Console.ReadLine();
        }
    }
}
