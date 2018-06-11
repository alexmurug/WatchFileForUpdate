using log4net;
using System;
using System.Configuration;
using System.IO;

namespace ReadTxtFile
{
    class MyService
    {
        
        public void Start()
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["DirectoryPath"];
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = ConfigurationManager.AppSettings["WatchFileName"];

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            //
        }

        //This method is called when a file is created, changed, or deleted.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            ILog logger = LogManager.GetLogger(typeof(MyService));

            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;

            //Send plaintext
            logger.Info(string.Format("File {0} {1} at {2}", Path.GetDirectoryName(e.FullPath), wct.ToString(), DateTime.Now.Ticks));

            string fileName = Path.GetFileNameWithoutExtension(e.FullPath);
            Path.GetPathRoot(e.FullPath);
            if (fileName == "NowPlaying")
            {
                StreamReader sr = null;
                var fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                try
                {
                    sr = new StreamReader(fs);
                    string[] Array = null;
                    string text = sr.ReadToEnd();
                    if (text.Length > 0)
                    {
                        Array = text.Split(':');
                    }
                    if(Array != null)
                    {
                        var letterCase = ConfigurationManager.AppSettings["LetterCase"];
                        if (Array.Length > 0)
                            File.WriteAllText(Path.GetDirectoryName(e.FullPath) + @"\Artist.txt", letterCase == "U" ? Array[0].ToUpper() : letterCase == "L" ? Array[0].ToLower() : Array[0]);

                        if (Array.Length > 1)
                            File.WriteAllText(Path.GetDirectoryName(e.FullPath) + @"\Title.txt", letterCase == "U" ? Array[1].ToUpper() : letterCase == "L" ? Array[1].ToLower() : Array[1]);
                    }
                    else
                    {
                        logger.Info(string.Format("Array has null at {0}", DateTime.Now));
                    }

                    sr.Close();
                }
                catch (FileNotFoundException fnfex)
                {
                    //Send an exception
                    logger.Error("File Not Found", fnfex);
                    Console.WriteLine("The file cannot be found.");
                }
                catch (IOException ioex)
                {
                    //Send an exception
                    logger.Error("IOExcepton", ioex);
                    Console.WriteLine("An I/O error has occurred.");
                }
                catch (OutOfMemoryException ofMex)
                {
                    //Send an exception
                    logger.Error("OutOfMemoryException", ofMex);
                    Console.WriteLine("There is insufficient memory to read the file.");
                }
                finally
                {
                    if (sr != null) sr.Dispose();
                }
            }
        }
    }
}
