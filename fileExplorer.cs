using Illustrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace IllustratorMagentoConsole
{
    internal class fileExplorer
    {
        private static List<string> pageSizes = new List<string> { "a3", "a4", "a6" };


        public class Folder
        {
            public string Path { set; get; }
            public List<FileInfo> Files { set; get; }
            //public List<string> Files { set; get; }

            public List<Folder> SubFolders { set; get; }

        }

        public Folder GetFolderHierarchy(string root)
        {
            var folder = new Folder();
            var dir = new DirectoryInfo(root);

            folder.Path = root;
            folder.Files = dir.GetFiles().ToList();
            //folder.Files = dir.GetFiles().Select(x => x.FullName).ToList();

            folder.SubFolders = dir.GetDirectories().Select(x => GetFolderHierarchy(x.FullName)).ToList();

            return folder;

        }


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
                    png.filePageSize = pageSizes.Find(element => words.Contains(element));
                    png.filePath =
                        Constants.exportadosFolder + Path.DirectorySeparatorChar +
                        png.folderName + Path.DirectorySeparatorChar +
                        Constants.fundasFolder + Path.DirectorySeparatorChar +
                        png.productName + Constants.imageExtension;
                    png.fullName = info.FullName;
                    var task = Task.Run(() =>
                    {
                        //string fullName, string folderName, string size, string exportName = ""
                        return ai.exportAiToPng(png.fullName, png.folderName, png.filePageSize, png.productName);
                    });

                    bool success = task.Wait(TimeSpan.FromMilliseconds(6000));

                    if (success)
                    {
                        return png;
                    }
                    else
                    {
                        Console.WriteLine("NO exportado POR TIMEOUT:" + png.folderName + '|' + png.productName + '|' + "filePageSize:" + png.filePageSize);
                        return null;
                    }
                }
            }
            return null;
        }

    }
}
