using System;
using System.IO;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Search.Storage
{
    public class FileHelper
    {
        private const int LastWriteTimePrecision = 2000;

        public static void Synchronize(DirectoryInfo source, DirectoryInfo destination, bool smart)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }
            FileInfo[] sources = source.GetFiles();
            ISet<string> srcNames = new HashedSet<string>();
            foreach (FileInfo fileInfo in sources)
            {
                srcNames.Add(fileInfo.Name);
            }
            FileInfo[] dests = destination.GetFiles();

            //delete files not present in source
            foreach (FileInfo file in dests)
            {
                if (!srcNames.Contains(file.Name))
                {
                    file.Delete();
                }
            }
            //copy each file from source
            foreach (FileInfo sourceFile in sources)
            {
                FileInfo destinationFile = new FileInfo(Path.Combine(destination.FullName,sourceFile.Name));
                long destinationChanged = destinationFile.LastWriteTime.Ticks/LastWriteTimePrecision;
                long sourceChanged = sourceFile.LastWriteTime.Ticks/LastWriteTimePrecision;
                if(!smart || destinationChanged != sourceChanged)
                {
                    sourceFile.CopyTo(destinationFile.FullName, true);
                }
            }

            foreach (DirectoryInfo directoryInfo in source.GetDirectories())
            {
                Synchronize(directoryInfo, 
                            new DirectoryInfo(Path.Combine(destination.FullName, directoryInfo.Name)), 
                            smart);
            }
        }
    }
}