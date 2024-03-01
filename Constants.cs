﻿using System;
using System.Collections.Generic;

namespace IllustratorMagentoConsole
{
    internal static class Constants
    {
        public static string exportadosFolder => "C:\\Users\\Jonatan\\Documents\\Public\\Manganimeshon\\DTF-UV\\2_Exportados"; //"E:\\Publico\\playeras a3\\Diseños Illustrator\\2_Exportados";
        public static string fundasFolder => "fundas"; //"E:\\Publico\\playeras a3\\Diseños Illustrator\\2_Exportados";
        public static string imageExtension => ".png"; //"E:\\Publico\\playeras a3\\Diseños Illustrator\\2_Exportados";
        public static string aiExtension => ".ai"; //"E:\\Publico\\playeras a3\\Diseños Illustrator\\2_Exportados";

        public static string clasificadosFolder = "C:\\Users\\Jonatan\\Documents\\Public\\Manganimeshon\\DTF-UV\\1_Clasificados"; // "E:\\Publico\\playeras a3\\Diseños Illustrator\\1_Clasificados";

        // public static string diseñosManganimeshon = "C:\\Users\\Jonatan\\Documents\\1_Public\\1_Manganimeshon\\1_DTF-UV\\3_Marcas\\KPOP\\BTS\\August D\\Agustina";

        public static string diseñosManganimeshon = "C:\\Users\\Jonatan\\Documents\\1_Public\\1_Manganimeshon\\1_DTF-UV\\3_Marcas";
        //public static string diseñosManganimeshon = "C:\\Users\\Jonatan\\Documents\\1_Public\\1_Manganimeshon\\1_DTF-UV\\4_Prueba";


        public static List<string> pageSizes = new List<string> { "a3", "a4", "a6" };
        public static HashSet<string> ExtensionWhitelist { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".ai" };

        public static class Magento
        {
            public static string baseUrl { get; set; } = "https://vulture-outgoing-oddly.ngrok-free.app";//"https://curzar.com"; https://vulture-outgoing-oddly.ngrok-free.app http://localhost:8080
            public static string username { get; set; } = "user"; //admin
            public static string password { get; set; } = "bitnami1"; //o48098Eh
        }

        public static class Shopify
        {
            public static string url { get; set; } = "curzar.myshopify.com";

            public static string token { get; set; } = "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";
        }


    }
}
