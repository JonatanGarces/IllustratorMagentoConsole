using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IllustratorMagentoConsole
{
    internal class fileExplorer
    {


        public class Folder
        {
            public string Path { set; get; }
            public string Name { set; get; }
            public List<File> Files { set; get; }
            public List<Folder> SubFolders { set; get; }

        }

        public class File
        {
            public string Path { set; get; }
            public string Name { set; get; }
            public string Extension { set; get; }
        }

        public Folder setFolderHierarchy(string root)
        {
            Folder folder = new Folder();
            DirectoryInfo dir = new DirectoryInfo(root);
            folder.Name = dir.Name;
            folder.Path = root;
            folder.Files = dir.GetFiles().Where(fn => Constants.ExtensionWhitelist.Contains(fn.Extension)).Select(x => getFileInFolder(x.FullName)).ToList();
            folder.SubFolders = dir.GetDirectories().Select(x => setFolderHierarchy(x.FullName)).ToList();
            return folder;
        }

        public File getFileInFolder(string name)
        {
            File file = new File();
            FileInfo info = new FileInfo(name);
            file.Name = info.Name;
            file.Extension = info.Extension;
            file.Path = info.FullName;
            return file;
        }




    }
}
