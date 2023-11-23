using Illustrator;
using ImageProcessor;
using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IllustratorMagentoConsole
{
    internal class illustratorHelper
    {
        public string fundaBackground = "C:\\Users\\Jonatan\\Documents\\Public\\Manganimeshon\\DTF-UV\\funda_transparente.png";
        public string exportadosFolder = "C:\\Users\\Jonatan\\Documents\\Public\\Manganimeshon\\DTF-UV\\2_Exportados"; //"E:\\Publico\\playeras a3\\Diseños Illustrator\\2_Exportados";

        private Application illuApp { get; set; }

        public void extractAiInformation(string filePath)
        {
            Application illuApp = new Application();
            OpenOptions openOptions = new OpenOptions();
            Document illuDoc = illuApp.Open(filePath, AiDocumentColorSpace.aiDocumentCMYKColor, openOptions);
            illuDoc.Activate();
            //Artboards artBoards = illuDoc.Artboards;

            var layers = illuDoc.Layers as List<Layer>;
            Layer layer = layers.Find(l => l.Name == "producto");
            if (layer != null)
            {
                foreach (TextFrame placedItem in layer.TextFrames)
                {
                    Console.WriteLine(placedItem.Name);
                    Console.WriteLine(placedItem.Contents);
                }
            }
        }


        public bool exportAiToPng(string fullName, string folderName, string exportName = "")
        {
            //Prepare paths and file names
            string exportFolder = exportadosFolder + Path.DirectorySeparatorChar + folderName;
            string pngFile = exportFolder + Path.DirectorySeparatorChar + exportName + ".png";
            string fundaFile = exportFolder + Path.DirectorySeparatorChar + "fundas" + Path.DirectorySeparatorChar + exportName + ".png";
            if (File.Exists(exportFolder))
            {
                return true;
            }
            ImageFactory imageFactory = new ImageFactory();
            bool isHorizontal = false;

            if (!File.Exists(pngFile))
            {
                string name = Path.GetFileNameWithoutExtension(fullName);
                if (illuApp == null)
                {
                    illuApp = new Application(); Thread.Sleep(500);
                }
                OpenOptions openOptions = new OpenOptions();
                Document illuDoc = illuApp.Open(fullName, AiDocumentColorSpace.aiDocumentCMYKColor, openOptions);
                illuDoc.Activate();

                //System.Threading.Thread.Sleep(800);
                double height = Math.Truncate(illuDoc.Height);
                double width = Math.Truncate(illuDoc.Width);

                if (!Directory.Exists(exportFolder)) { Directory.CreateDirectory(exportFolder); Thread.Sleep(500); }
                //if (System.IO.File.Exists(pngFile + ".png")) { System.IO.File.Delete(pngFile + ".png"); System.Threading.Thread.Sleep(800); };
                ExportForScreensOptionsPNG24 exportOptions = new ExportForScreensOptionsPNG24();
                exportOptions.Transparency = true;

                //con esto volteas la imagen
                isHorizontal = width > height;
                if (isHorizontal)
                {
                    exportOptions.ScaleType = AiExportForScreensScaleType.aiScaleByWidth;
                }
                else
                {
                    exportOptions.ScaleType = AiExportForScreensScaleType.aiScaleByHeight;
                }
                exportOptions.ScaleTypeValue = 380;
                exportOptions.AntiAliasing = AiAntiAliasingMethod.aiArtOptimized;
                exportOptions.BackgroundBlack = false;
                exportOptions.Interlaced = false;
                ExportForScreensItemToExport itemToExport = new ExportForScreensItemToExport();
                itemToExport.Document = false;

                Artboards artBoards = illuDoc.Artboards;
                string artBoardName = "";
                foreach (Artboard artBoard in artBoards)
                {
                    try
                    {
                        if (artBoard.Name == null)
                        {
                            continue;
                        }
                        artBoardName = artBoard.Name;
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                illuDoc.Artboards.GetByName(artBoardName).Name = exportName;
                illuDoc.ExportForScreens(exportFolder, AiExportForScreensType.aiSE_PNG24, exportOptions, itemToExport, "");
                string artBoardFile = exportFolder + Path.DirectorySeparatorChar + "PNG" + Path.DirectorySeparatorChar + exportName + ".png";
                WhenFileCreated(artBoardFile);
                //Illustrator.IllustratorSaveOptions illustratorSaveOptions = new Illustrator.IllustratorSaveOptions();
                illuDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
                // if (System.IO.File.Exists(artBoardFile)) { MoveFile(artBoardFile, pngFile); };
                TextLayer textLayer = new TextLayer();
                textLayer.Text = "manganimeshon.com";
                textLayer.FontSize = 25;
                textLayer.DropShadow = true;
                textLayer.FontColor = Color.White;
                textLayer.Opacity = 50;
                imageFactory.Load(artBoardFile).Watermark(textLayer).Save(pngFile);
                DeleteFileAndWait(artBoardFile);

            }

            imageFactory = new ImageFactory();
            ImageLayer layer = new ImageLayer();
            ImageFactory imageFactory1 = new ImageFactory();
            Image image = null;
            image = imageFactory.Load(pngFile).Image;
            if (image.Size.Width > image.Size.Height)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            layer.Image = image;
            Size layerSize = new Size();
            layerSize.Height = 238;
            layer.Size = layerSize;

            Point point = new Point();
            point.X = 51;
            point.Y = 99;
            layer.Position = point;

            imageFactory.Load(fundaBackground).Overlay(layer).Save(fundaFile);

            return true;
        }

        private void DeleteFileAndWait(string filepath, int timeout = 30000)
        {
            using (var fw = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath)))
            using (var mre = new ManualResetEventSlim())
            {
                fw.EnableRaisingEvents = true;
                fw.Deleted += (sender, e) =>
                {
                    mre.Set();
                };
                File.Delete(filepath);
                mre.Wait(timeout);
            }
        }

        private async void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                        sourceStream.Close();
                        File.Delete(sourceFile);
                    }
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine("An IOException occured during move, " + ioex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception occured during move, " + ex.Message);
            }
        }

        public static Task WhenFileCreated(string path)
        {
            if (File.Exists(path))
                return Task.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();
            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(path));

            FileSystemEventHandler createdHandler = null;
            RenamedEventHandler renamedHandler = null;
            createdHandler = (s, e) =>
            {
                if (e.Name == Path.GetFileName(path))
                {
                    tcs.TrySetResult(true);
                    watcher.Created -= createdHandler;
                    watcher.Dispose();
                }
            };

            renamedHandler = (s, e) =>
            {
                if (e.Name == Path.GetFileName(path))
                {
                    tcs.TrySetResult(true);
                    watcher.Renamed -= renamedHandler;
                    watcher.Dispose();
                }
            };

            watcher.Created += createdHandler;
            watcher.Renamed += renamedHandler;
            watcher.EnableRaisingEvents = true;

            return tcs.Task;
        }

        //Artboard workTable = illuDoc.Artboards;
        //illuDoc.SelectObjectsOnActiveArtboard
        //illuDoc.Artboards.GetByName(artBoardName).Name = exportName;
        /* foreach (Artboard artBoard in artBoards)
         {
             try
             {
                 if (artBoard.Name == "Funda")
                 {
                     artBoardName = artBoard.Name;

                     artBoardIndex = index;
                     break;
                 }

             }
             catch (Exception e)
             {
             }
             index++;

         }

         Console.WriteLine(artBoardName);
         Console.WriteLine(artBoardIndex);
        */
        //artBoards.SetActiveArtboardIndex(artBoardIndex);
        //int  getActiveIndex = artBoards.GetActiveArtboardIndex();
        //illuDoc.Se;
        //illuDoc.
        //Console.WriteLine(getActiveIndex);

        /*
        ExportOptionsPNG24 exportOptions = new ExportOptionsPNG24();
        exportOptions.Transparency = true;
        exportOptions.AntiAliasing = true;
        exportOptions.VerticalScale = 100;
        exportOptions.HorizontalScale = 100;
        exportOptions.ArtBoardClipping = true;
        exportOptions.Matte = false;
        exportOptions.SaveAsHTML = false;
        illuDoc.Export(pngFile, Illustrator.AiExportType.aiPNG24, exportOptions);
        */



        /*
                 public string[] getFolders()
        {
            string[] allFolders = Directory.GetDirectories(clasificadosFolder, "*.*", SearchOption.AllDirectories);
            return allFolders;
        }
         public void createAiToPng()
        {
            List<String> aiFiles = getAiFiles(getFolders());
            foreach (var file in aiFiles)
            {
                FileInfo info = new FileInfo(file);
                exportAiToPng(info.FullName, info.Directory.Name, "");
            }
        }

        public List<String> getAiFiles(string[] allFolders)
        {
            List<string> allFiles = new List<string>();
            foreach (var folder in allFolders)
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                foreach (var file in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories))
                {
                    allFiles.Add(file);
                }
            }
            return allFiles;
        }
         */
    }

}
